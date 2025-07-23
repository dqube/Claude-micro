using Microsoft.AspNetCore.Http;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class CorrelationHelper
{
    public static string GetOrCreateCorrelationId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        if (httpContext.Request.Headers.TryGetValue(HeaderConstants.CorrelationId, out var correlationId) 
            && !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        var newCorrelationId = GenerateCorrelationId();
        httpContext.Request.Headers[HeaderConstants.CorrelationId] = newCorrelationId;
        httpContext.Response.Headers[HeaderConstants.CorrelationId] = newCorrelationId;
        
        return newCorrelationId;
    }

    public static string GetCorrelationId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        return httpContext.Request.Headers.TryGetValue(HeaderConstants.CorrelationId, out var correlationId)
            ? correlationId.ToString()
            : httpContext.TraceIdentifier;
    }

    public static void SetCorrelationId(HttpContext httpContext, string correlationId)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(correlationId);
        
        httpContext.Request.Headers[HeaderConstants.CorrelationId] = correlationId;
        httpContext.Response.Headers[HeaderConstants.CorrelationId] = correlationId;
    }

    public static string GenerateCorrelationId()
    {
        return $"CID-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }

    public static string GetRequestId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        return httpContext.Request.Headers.TryGetValue(HeaderConstants.RequestId, out var requestId)
            ? requestId.ToString()
            : httpContext.TraceIdentifier;
    }

    public static void SetRequestId(HttpContext httpContext, string requestId)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(requestId);
        
        httpContext.Request.Headers[HeaderConstants.RequestId] = requestId;
        httpContext.Response.Headers[HeaderConstants.RequestId] = requestId;
    }

    public static string GenerateRequestId()
    {
        return $"REQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }

    public static IDictionary<string, object> GetTrackingContext(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        return new Dictionary<string, object>
        {
            ["CorrelationId"] = GetCorrelationId(httpContext),
            ["RequestId"] = GetRequestId(httpContext),
            ["TraceId"] = httpContext.TraceIdentifier,
            ["Timestamp"] = DateTime.UtcNow,
            ["Method"] = httpContext.Request.Method,
            ["Path"] = httpContext.Request.Path.Value ?? string.Empty,
            ["UserAgent"] = httpContext.Request.Headers.UserAgent.ToString()
        };
    }
}