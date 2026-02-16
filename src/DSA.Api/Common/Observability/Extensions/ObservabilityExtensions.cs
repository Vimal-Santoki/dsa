using System.Reflection;
using DSA.Api.Common.Observability.Diagnostics;
using DSA.Api.Common.Observability.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DSA.Api.Common.Observability.Extensions
{
    internal static class ObservabilityExtensions
    {
        private const string ServiceName = "DSA.Api";

        public static void AddObservability(this IHostApplicationBuilder builder)
        {
            // 1. Get Version from the Assembly (populated by GitHub Actions /p:Version=...)
            var assembly = Assembly.GetEntryAssembly();
            var serviceVersion = assembly?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?? assembly?.GetName().Version?.ToString()
                ?? "unknown";

            // 2. Define the "Resource" (The "Identity" of this service in the mesh)
            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(serviceName: ServiceName, serviceVersion: serviceVersion)
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector();

            // 3. Configure OpenTelemetry
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .SetResourceBuilder(resourceBuilder)
                        .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)))
                        .AddSource(AppDiagnostics.ActivitySourceName) // Manual tracing source
                        .AddAspNetCoreInstrumentation() // Auto-trace Controllers
                        .AddHttpClientInstrumentation() // Auto-trace outgoing HTTP
                        .AddOtlpExporter();             // Ship to Docker (Collector)
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .SetResourceBuilder(resourceBuilder)
                        .SetExemplarFilter(ExemplarFilterType.TraceBased)
                        .AddMeter(AppMetrics.MeterName)
                        .AddRuntimeInstrumentation()       // GC, Memory, CPU
                        .AddAspNetCoreInstrumentation()    // Request Counts, Latency
                        .AddHttpClientInstrumentation()    // Outgoing HTTP Latency
                        .AddOtlpExporter();              // Ship to Docker (Collector)

                });

            // 4. Configure Logging (The "L" in LGTM)
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeScopes = true;
                logging.SetResourceBuilder(resourceBuilder);
                logging.AddOtlpExporter(); // Sends logs to the same Collector
                logging.IncludeFormattedMessage = true;
                logging.ParseStateValues = true;
            });
        }
    }
}
