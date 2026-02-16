using Microsoft.Extensions.Logging;

namespace DSA.Api.Common.Extensions
{
    internal static class LoggerExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Performance wrapper for LogDebug")]
        public static void LogDebugSafe(this ILogger logger, string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(message, nameof(message));
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(message, args);
            }
        }
    }
}
