using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log incoming request
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("Incoming Request: {Method} {Path}", method, path);

        await _next(context);

        // Log outgoing response
        var statusCode = context.Response.StatusCode;
        _logger.LogInformation("Outgoing Response: {StatusCode}", statusCode);
    }
}
