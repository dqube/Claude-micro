using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Middleware.Security;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitingOptions _options;
    private readonly ConcurrentDictionary<string, ClientRequestInfo> _clients;
    private readonly Timer _cleanupTimer;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        ILogger<RateLimitingMiddleware> logger,
        RateLimitingOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
        _clients = new ConcurrentDictionary<string, ClientRequestInfo>();
        
        // Cleanup expired entries every minute
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;
        
        var clientInfo = _clients.AddOrUpdate(clientId, 
            new ClientRequestInfo { FirstRequestTime = now, RequestCount = 1 },
            (key, existing) =>
            {
                // Reset if window has passed
                if (now - existing.FirstRequestTime >= _options.Window)
                {
                    return new ClientRequestInfo { FirstRequestTime = now, RequestCount = 1 };
                }
                
                existing.RequestCount++;
                return existing;
            });

        if (clientInfo.RequestCount > _options.PermitLimit)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}. Count: {RequestCount}, Limit: {PermitLimit}", 
                clientId, clientInfo.RequestCount, _options.PermitLimit);

            context.Response.StatusCode = 429;
            context.Response.Headers.TryAdd("Retry-After", _options.Window.TotalSeconds.ToString());
            
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get client IP from various headers (for proxy scenarios)
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

        return ip;
    }

    private void CleanupExpiredEntries(object? state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _clients
            .Where(kvp => now - kvp.Value.FirstRequestTime >= _options.Window)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _clients.TryRemove(key, out _);
        }

        _logger.LogDebug("Cleaned up {ExpiredCount} expired rate limiting entries", expiredKeys.Count);
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

public class ClientRequestInfo
{
    public DateTime FirstRequestTime { get; set; }
    public int RequestCount { get; set; }
}