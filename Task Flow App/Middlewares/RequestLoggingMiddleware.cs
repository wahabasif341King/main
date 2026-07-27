// Middleware/RequestLoggingMiddleware.cs
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.Now;
        _logger.LogInformation($"Request Start: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        var duration = (DateTime.Now - start).TotalMilliseconds;
        _logger.LogInformation($"Request End: {context.Response.StatusCode} ({duration}ms)");
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}