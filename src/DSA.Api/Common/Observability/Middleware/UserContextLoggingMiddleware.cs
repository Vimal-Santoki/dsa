using System.Diagnostics;
using System.Security.Claims;

namespace DSA.Api.Common.Observability.Middleware
{
    internal class UserContextLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserContextLoggingMiddleware> _logger;

        public UserContextLoggingMiddleware(RequestDelegate next, ILogger<UserContextLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            // 1. Identify the user (using 'sub' claim is standard OIDC/OAuth practice)
            // If identity name is available, use it, otherwise fall back to 'sub' claim, or anonymous
             var userId = user.Identity?.Name 
                         ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? "anonymous";

            // 2. Add to Activity (Trace)
            // User requested 'user.Id' instead of 'enduser.id'
            Activity.Current?.SetTag("user.Id", userId);

            // 3. Add to Logs (Scope)
            // We create a scope with a dictionary of tags.
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["user.Id"] = userId
            }))
            {
                // Everything logged inside this 'using' block will have the user.Id attached
                await _next(context);
            }
        }
    }
}
