using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DSA.Api.Shared
{
    internal sealed partial class GlobalExceptionHandler : IExceptionHandler
    {
        readonly ILogger<GlobalExceptionHandler> _logger;

        // High Performance Logger Definition
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Error,
            Message = "An unhandled exception occurred. Message: {message}")]
        private static partial void LogUnhandledException(ILogger logger, string message, Exception exception);

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            LogUnhandledException(_logger, exception.Message, exception);

            var problemDetails = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message,
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true; // exception was handled.
        }
    }
}
