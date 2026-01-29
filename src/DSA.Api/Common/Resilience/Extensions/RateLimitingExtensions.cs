using System.Threading.RateLimiting;
using DSA.Api.Common.Resilience.Dto;
using Microsoft.Extensions.Options;

namespace DSA.Api.Common.Resilience.Extensions
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

                options.OnRejected = async (context, token) =>
                {
                    // FIX: Manually tag the OpenTelemetry Activity so metrics see the 429
                    if (System.Diagnostics.Activity.Current is { } activity)
                    {
                        activity.SetTag("http.response.status_code", 429);
                        activity.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Rate Limit Exceeded");
                    }
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<RateLimiter>>();
                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    
                    logger?.LogWarning("Rate limit exceeded for IP {Ip} on {Path}", ip, context.HttpContext.Request.Path);

                    // Rejection logic duplicated from default if we override OnRejected? 
                    // No, default behavior is overridden.
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                };
            });
        }

        public static void UseRateLimiting(this WebApplication app)
        {
            app.UseRateLimiter();
        }
    }
}
