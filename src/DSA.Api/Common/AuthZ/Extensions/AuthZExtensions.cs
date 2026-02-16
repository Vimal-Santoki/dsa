
using DSA.Api.Common.AuthZ.Dto;
using DSA.Api.Common.AuthZ.Filters;
using DSA.Api.Common.AuthZ.Services;
using Microsoft.AspNetCore.Authorization;

namespace DSA.Api.Common.AuthZ.Extensions
{
    internal static class AuthZExtensions
    {
        public static void AddAuthZ(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Sorting:List", policy => policy.AddRequirements(new PermissionRequirement("Sorting:List")));
                options.AddPolicy("Sorting:Execute", policy => policy.AddRequirements(new PermissionRequirement("Sorting:Execute")));
            });
        }

        public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string action, string resource)
        {
            return builder.AddEndpointFilter(new EndpointPermissionFilter(action, resource));
        }

        public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string action, RouteParam routeParam)
        {
            return builder.AddEndpointFilter(new EndpointPermissionFilter(action, null, routeParam.Name));
        }
    }
}
