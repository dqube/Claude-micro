using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using BuildingBlocks.API.Configuration.Options;
using System.Collections.Concurrent;
using System.Net;

namespace BuildingBlocks.API.Middleware.Security;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitingOptions _options;
    private readonly ConcurrentDictionary<string, RateLimitEntry> _rateLimitStore;

    public RateLimitingMiddleware(RequestDelegate next, IOptions<RateLimitingOptions> options)
    {
        _next = next;
        _options = options.Value;
        _rateLimitStore = new ConcurrentDictionary<string, RateLimitEntry>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var endpoint = GetEndpoint(context);
        
        // Get rate limit settings for this endpoint or use default
        var rateLimitSettings = GetRateLimitSettings(endpoint);
        
        if (!IsRequestAllowed(clientId, rateLimitSettings))
        {
            await HandleRateLimitExceeded(context, rateLimitSettings);
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get client IP
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        
        // Check for forwarded IP headers
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        else if (context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            clientIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        return clientIp ?? "unknown";
    }

    private string GetEndpoint(HttpContext context)
    {
        return $"{context.Request.Method}:{context.Request.Path}";
    }

    private RateLimitSettings GetRateLimitSettings(string endpoint)
    {
        if (_options.EndpointPolicies.TryGetValue(endpoint, out var endpointPolicy))
        {
            return new RateLimitSettings
            {
                PermitLimit = endpointPolicy.PermitLimit,
                Window = endpointPolicy.Window,
                Policy = endpointPolicy.Policy
            };
        }

        return new RateLimitSettings
        {
            PermitLimit = _options.PermitLimit,
            Window = _options.Window,
            Policy = _options.Policy
        };
    }

    private bool IsRequestAllowed(string clientId, RateLimitSettings settings)
    {
        var key = $"{clientId}:{DateTime.UtcNow.Ticks / settings.Window.Ticks}";
        
        var entry = _rateLimitStore.AddOrUpdate(key, 
            new RateLimitEntry { Count = 1, WindowStart = DateTime.UtcNow },
            (k, existing) => 
            {
                if (DateTime.UtcNow - existing.WindowStart > settings.Window)
                {
                    return new RateLimitEntry { Count = 1, WindowStart = DateTime.UtcNow };
                }
                return existing with { Count = existing.Count + 1 };
            });

        // Cleanup old entries periodically
        if (DateTime.UtcNow.Second == 0) // Every minute
        {
            CleanupOldEntries();
        }

        return entry.Count <= settings.PermitLimit;
    }

    private void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        var keysToRemove = _rateLimitStore
            .Where(kvp => kvp.Value.WindowStart < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _rateLimitStore.TryRemove(key, out _);
        }
    }

    private async Task HandleRateLimitExceeded(HttpContext context, RateLimitSettings settings)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/json";
        
        context.Response.Headers.Add("X-RateLimit-Limit", settings.PermitLimit.ToString());
        context.Response.Headers.Add("X-RateLimit-Remaining", "0");
        context.Response.Headers.Add("X-RateLimit-Reset", 
            ((DateTimeOffset)DateTime.UtcNow.Add(settings.Window)).ToUnixTimeSeconds().ToString());
        
        var response = new
        {
            success = false,
            message = "Rate limit exceeded",
            retryAfter = (int)settings.Window.TotalSeconds,
            timestamp = DateTime.UtcNow
        };
        
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private record RateLimitEntry
    {
        public int Count { get; init; }
        public DateTime WindowStart { get; init; }
    }

    private record RateLimitSettings
    {
        public int PermitLimit { get; init; }
        public TimeSpan Window { get; init; }
        public RateLimitingPolicy Policy { get; init; }
    }
}