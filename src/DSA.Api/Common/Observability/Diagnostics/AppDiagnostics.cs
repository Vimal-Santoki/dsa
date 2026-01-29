using System.Diagnostics;
using System.Reflection;

namespace DSA.Api.Common.Observability.Diagnostics
{
    internal static class AppDiagnostics
    {
        // Unique name for this source
        public const string ActivitySourceName = "DSA.Algorithms";

        // The Source object used to start Activities
        public static readonly ActivitySource ActivitySource = new(
            ActivitySourceName,
            Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0");
    }
}
