using Polly;
using System.Threading.RateLimiting;

namespace DSA.Api.Common.Resilience.Extensions
{
    internal static class ResilienceExtensions
    {
        public const string CpuIntensivePipelineName = "CpuIntensive";

        public static void AddResiliencePipelines(this IHostApplicationBuilder builder)
        {
            builder.Services.AddResiliencePipeline(CpuIntensivePipelineName, pipelineBuilder =>
            {
                // SAFETY: Bulkhead Isolation
                // Only allow 2 concurrent executions of sorting logic.
                // Queue up to 4 requests waiting for a slot.
                // Anything else gets rejected immediately.
                pipelineBuilder.AddConcurrencyLimiter(new ConcurrencyLimiterOptions
                {
                    PermitLimit = 2,      // Very conservative for demo (simulates strict CPU budget)
                    QueueLimit = 4,       // Allow short burst
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        }
    }
}
