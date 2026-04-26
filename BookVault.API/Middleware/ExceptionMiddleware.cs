namespace BookVault.API.Middleware
{
    public class ExceptionMiddleware
    {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;

            public ExceptionMiddleware(RequestDelegate next,
                ILogger<ExceptionMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Resource not found");
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(
                        new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(
                        new { error = "Something went wrong" });
                }
            }
        
    }
}
