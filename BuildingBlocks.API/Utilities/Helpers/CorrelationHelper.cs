using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class CorrelationHelper
{
    public const string CorrelationIdHeaderName = "X-Correlation-ID";
    public const string CorrelationIdKey = "CorrelationId";

    public static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is already set in items
        if (context.Items.TryGetValue(CorrelationIdKey, out var existingId) && existingId is string correlationId)
        {
            return correlationId;
        }

        // Try to get from request header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var headerValue) && 
            !string.IsNullOrWhiteSpace(headerValue.FirstOrDefault()))
        {
            correlationId = headerValue.FirstOrDefault()!;
        }
        else
        {
            // Try to get from Activity (tracing)
            correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
        }

        // Store in context items for future use
        context.Items[CorrelationIdKey] = correlationId;
        
        // Add to response headers
        if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
        {
            context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
        }

        return correlationId;
    }

    public static string? GetCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdKey, out var correlationId) && correlationId is string id)
        {
            return id;
        }

        return context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();
    }

    public static void SetCorrelationId(HttpContext context, string correlationId)
    {
        context.Items[CorrelationIdKey] = correlationId;
        
        if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
        {
            context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
        }
    }

    public static string CreateCorrelationId()
    {
        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }

    public static void EnrichActivity(string correlationId)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("correlation.id", correlationId);
            activity.SetBaggage("correlation.id", correlationId);
        }
    }

    public static Dictionary<string, object> CreateCorrelationContext(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var traceId = Activity.Current?.TraceId.ToString();
        var spanId = Activity.Current?.SpanId.ToString();
        
        var correlationContext = new Dictionary<string, object>
        {
            ["correlationId"] = correlationId,
            ["timestamp"] = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(traceId))
        {
            correlationContext["traceId"] = traceId;
        }

        if (!string.IsNullOrEmpty(spanId))
        {
            correlationContext["spanId"] = spanId;
        }

        return correlationContext;
    }
}