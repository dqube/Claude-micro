using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Middleware.Logging;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Set the correlation ID in the response headers
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        
        // Store correlation ID in HttpContext items for easy access
        context.Items["CorrelationId"] = correlationId;

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is provided in request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) &&
            !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        // Use TraceIdentifier as correlation ID if none provided
        return context.TraceIdentifier;
    }
}