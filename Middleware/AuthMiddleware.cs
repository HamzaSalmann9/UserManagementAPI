public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    // Simple example of valid token
    private const string ValidToken = "TECHHIVE-SECURE-TOKEN";

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader is null || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Missing token.");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        if (token != ValidToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid token.");
            return;
        }

        await _next(context);
    }
}
