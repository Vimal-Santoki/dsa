using System.Diagnostics;
using DSA.Api.Common.Observability.Diagnostics;
using DSA.Api.Common.Observability.Metrics;
using DSA.Api.Common.Resilience.Extensions;
using DSA.Api.Features.Sorting.Interfaces;
using OpenTelemetry.Trace;
using Polly.Registry;

namespace DSA.Api.Features.Sorting.Algorithms.Decorators
{
    internal sealed class ObservabilitySortDecorator : ISortAlgorithm
    {
        private readonly ISortAlgorithm _inner;
        public ObservabilitySortDecorator(ISortAlgorithm inner)
        {
            _inner = inner;
        }
        public string Name => _inner.Name;

        public string Code => _inner.Code;

        public string Category => _inner.Category;

        public int Sort(int[] array)
        {
            var size = array.Length;

            var tags = new TagList
            {
                { "algorithm.name", Name },
                { "algorithm.code", Code },
                { "algorithm.category", Category }
            };
            using var activity = AppDiagnostics.ActivitySource.StartActivity("Sorting.Execution");
            activity?.SetTag("algorithm.name", Name);
            activity?.SetTag("algorithm.code", Code);
            activity?.SetTag("algorithm.category", Category);
            activity?.SetTag("data.input_size", size);

            var status = "success";
            var stopwatch = Stopwatch.StartNew();

            try
            {
                AppMetrics.InputSize.Record(size, tags);


                // Execute the actual Logic
                var result = _inner.Sort(array);

                activity?.SetTag("data.iterations", result);


                var successTags = tags;
                successTags.Add("status", status);
                AppMetrics.Executions.Add(1, successTags);

                return result;
            }
            catch (Exception ex)
            {
                status = "error";
                var errorTags = tags;
                errorTags.Add("status", status);
                AppMetrics.Executions.Add(1, errorTags);

                activity?.AddException(ex);
                activity?.SetStatus(ActivityStatusCode.Error);

                throw; // Rethrow so the global exception handler catches it
            }
            finally
            {
                stopwatch.Stop();

                var durationTags = tags;
                durationTags.Add("status", status);
                AppMetrics.DurationMs.Record(stopwatch.Elapsed.TotalMilliseconds, durationTags);

                activity?.SetTag("data.duration_ms", stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
