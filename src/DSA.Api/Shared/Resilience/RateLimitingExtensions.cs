using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace DSA.Api.Resilience
{
    internal static class RateLimitingExtensions
    {
        private const string HealthCheckPolicyName = "health-checks";

        public static void AddRateLimiting(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            services.Configure<RateLimitingSettings>(
                builder.Configuration.GetSection(RateLimitingSettings.SectionName));

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    // Resolve settings per request to support hot reload
                    var settings = httpContext.RequestServices.GetRequiredService<IOptionsSnapshot<RateLimitingSettings>>().Value;

                    // No rate limiting for health checks to allow monitoring systems to function properly
                    if (httpContext.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
                    {
                        return RateLimitPartition.GetNoLimiter(HealthCheckPolicyName);
                    }

                    // Use the client IP address as the partition key.
                    // TRUST: We assume Reponse.Connection.RemoteIpAddress is correct because 
                    // ForwardedHeadersMiddleware is configured in Program.cs
                    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    // Define a sliding window rate limiter for each client IP
                    return RateLimitPartition.GetSlidingWindowLimiter(clientIp, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowInSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        SegmentsPerWindow = settings.SegmentsPerWindow
                    });
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }

        public static void UseRateLimiting(this WebApplication app)
        {
            app.UseRateLimiter();
        }
    }
}
