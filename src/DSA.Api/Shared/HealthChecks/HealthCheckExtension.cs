using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DSA.Api.Shared.HealthChecks
{
    internal static class HealthCheckExtension
    {
        public static void AddAppHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]); // Liveness probe, indicates if the app is running

            //.AddSqlServer(
            //    connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
            //    name: "sqlserver",
            //    tags: ["ready"]); // Readiness probe, indicates if the pod can connect to the database

            //.AddRedis(
            //    redisConnectionString: builder.Configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("Connection string 'RedisConnection' not found."),
            //    name: "redis",
            //    tags: ["ready"]); // Readiness probe, indicates if the pod can connect to Redis

            //.AddPrivateMemoryHealthCheck(
            //    maximumMegabytes: 1024, // 1 GB
            //    name: "privatememory",
            //    tags: ["ready"]); // Readiness probe, indicates if the pod is within memory limits. This will prevent the pod accepting new requests if it's using too much memory. Allows to prevent OOM kills due to any memory leaks.

            // Additional health checks can be added here as application grows ie. external service dependencies
        }

        public static void MapAppHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("live")
            });
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("ready")
            });
        }
    }
}
