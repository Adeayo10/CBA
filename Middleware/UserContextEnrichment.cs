using System.Diagnostics;
using System.Security.Claims;

namespace CBA.Middlewares
{
    public sealed class UserContextEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserContextEnrichmentMiddleware> _logger;
        public UserContextEnrichmentMiddleware(RequestDelegate next, ILogger<UserContextEnrichmentMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId is not null)
            {
                Activity.Current?.AddTag("userId", userId);
                var data = new Dictionary<string, string>
                {
                    {"userId", userId}
                };
                
                using (_logger.BeginScope(data))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
            
        }
    }
}