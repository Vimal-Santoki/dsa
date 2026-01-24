using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DSA.Api.Common.Health.Api
{
    internal static class HealthEndpoints
    {
        public static void MapHealthCheckEndpoints(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("live")
            }).AllowAnonymous();
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = (check) => true // Include all checks for readiness
            }).AllowAnonymous();
        }
    }
}
