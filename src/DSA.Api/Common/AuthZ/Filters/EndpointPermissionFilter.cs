
using System.Security.Claims;
using DSA.Api.Common.Iam.Interfaces;

namespace DSA.Api.Common.AuthZ.Filters
{
    internal class EndpointPermissionFilter : IEndpointFilter
    {
        readonly string _action;
        readonly string? _resource;
        readonly string? _routeParamName;
        public EndpointPermissionFilter(string action, string? resource = null, string? routeParamName = null)
        {
            _action = action;
            _resource = resource;
            _routeParamName = routeParamName;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var resourceId = _resource;
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true) return Results.Unauthorized();

            // If a route parameter name is provided, extract its value to use as the resource ID
            if (_routeParamName != null)
            {
                var routeValue = context.HttpContext.GetRouteValue(_routeParamName);
                if (routeValue is null)
                {
                    return Results.BadRequest($"Resource identifier missing in request path.");
                }
                resourceId = routeValue.ToString();
            }
            if (string.IsNullOrEmpty(resourceId)) return Results.Forbid();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Results.Forbid();

            var iamService = context.HttpContext.RequestServices.GetRequiredService<IIamService>();
            var isAuthorized = await iamService.IsAuthorizedAsync(userId, _action, resourceId!);
            if (!isAuthorized) return Results.Forbid();

            return await next(context);
        }
    }
}
