using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Modique.API.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication middleware for public endpoints
        if (IsPublicEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Check if user is authenticated (JWT middleware should have already validated the token)
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Extract user information from claims for logging/debugging
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            _logger.LogDebug("Authenticated request: UserId={UserId}, Email={Email}, Role={Role}, Path={Path}",
                userId, email, role, context.Request.Path);

            // You can add additional authorization checks here if needed
            // For example, check if user is active, check role-based access, etc.
        }

        await _next(context);
    }

    private static bool IsPublicEndpoint(PathString path)
    {
        var publicPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/register",
            "/swagger",
            "/api/swagger"
        };

        return publicPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }
}
