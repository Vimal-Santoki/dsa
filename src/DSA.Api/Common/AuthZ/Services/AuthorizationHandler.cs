using System.Security.Claims;
using DSA.Api.Common.Extensions;
using DSA.Api.Common.Iam.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace DSA.Api.Common.AuthZ.Services
{
    // The "Question" we ask: "Does user have permission for Action X?"
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Action { get; }
        public PermissionRequirement(string action) => Action = action;
    }

    internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly IIamService _iamService;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(IIamService iamService, ILogger<PermissionAuthorizationHandler> logger)
        {
            _iamService = iamService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogDebugSafe("Authorization Handled: User ID not found in claims.");
                return;
            }

            // Check against Global Resource (*) by default for Attributes.
            // For dynamic resource checks, imperative 'AuthorizeAsync' is used with resource string.
            var resource = context.Resource as string ?? "*";

            if (await _iamService.IsAuthorizedAsync(userId, requirement.Action, resource))
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Policy requirement failed for User {UserId} on Action {Action}", userId, requirement.Action);
            }
        }
    }

}
