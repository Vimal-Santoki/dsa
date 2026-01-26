using System.Security.Claims;
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
        public PermissionAuthorizationHandler(IIamService iamService)
        {
            _iamService = iamService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return;

            // Check against Global Resource (*) by default for Attributes.
            // For dynamic resource checks, imperative 'AuthorizeAsync' is used with resource string.
            var resource = context.Resource as string ?? "*";

            if (await _iamService.IsAuthorizedAsync(userId, requirement.Action, resource))
            {
                context.Succeed(requirement);
            }
        }
    }

}
