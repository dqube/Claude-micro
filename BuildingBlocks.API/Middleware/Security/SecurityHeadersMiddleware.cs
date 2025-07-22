using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.API.Middleware.Security;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers
        var headers = context.Response.Headers;

        // Prevent MIME type sniffing
        headers.TryAdd("X-Content-Type-Options", "nosniff");

        // Prevent clickjacking attacks
        headers.TryAdd("X-Frame-Options", "DENY");

        // Enable XSS protection
        headers.TryAdd("X-XSS-Protection", "1; mode=block");

        // Control referrer information
        headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");

        // Content Security Policy
        headers.TryAdd("Content-Security-Policy", "default-src 'self'");

        // Prevent browsers from caching sensitive content
        headers.TryAdd("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
        headers.TryAdd("Pragma", "no-cache");

        // Remove server identification
        headers.Remove("Server");

        await _next(context);
    }
}