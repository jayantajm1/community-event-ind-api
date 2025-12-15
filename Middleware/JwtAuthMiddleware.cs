namespace CommunityEventsApi.Middleware;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Implement JWT token validation
        // Extract token from Authorization header
        // Validate token
        // Set user claims in context

        await _next(context);
    }
}
