using System.Diagnostics.Metrics;
using System.Reflection;

namespace DSA.Api.Common.Observability.Metrics
{
    internal static class AppMetrics
    {
        // The "Channel" name.
        public const string MeterName = "DSA.Algorithms";

        private static readonly Meter Meter = new(MeterName,
            Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0");

        // Histogram: Tracks the size of the input (e.g., array length)
        public static readonly Histogram<int> InputSize = Meter.CreateHistogram<int>(
            "dsa.algorithm.input_size",
            "items",
            "The size of the input data structure");

        // Counter: Tracks how many times an algorithm runs
        public static readonly Counter<long> Executions = Meter.CreateCounter<long>(
            "dsa.algorithm.executions",
            "ops",
            "Total number of algorithm executions");

        // duration histogram: Tracks the duration of algorithm execution
        public static readonly Histogram<double> DurationMs = Meter.CreateHistogram<double>(
            "dsa.algorithm.duration_ms",
            "ms",
            "Duration of algorithm execution in milliseconds");
    }
}
