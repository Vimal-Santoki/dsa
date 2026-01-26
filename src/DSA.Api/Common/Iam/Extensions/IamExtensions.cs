using DSA.Api.Common.Iam.Interfaces;
using DSA.Api.Common.Iam.Services;
using Microsoft.AspNetCore.Authorization;

namespace DSA.Api.Common.Iam.Extensions
{
    internal static class IamExtensions
    {
        public static void AddIam(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPolicyEvaluator, PolicyEvaluator>();
            builder.Services.AddSingleton<IPermissionService, MockPermissionService>();
            builder.Services.AddSingleton<IIamService, IamService>();
        }
    }
}
