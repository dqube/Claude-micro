using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.API.Utilities.Constants;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Extensions;

public static class HttpContextExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    public static string GetCorrelationId(this HttpContext context)
    {
        return CorrelationHelper.GetCorrelationId(context);
    }

    public static string GetOrCreateCorrelationId(this HttpContext context)
    {
        return CorrelationHelper.GetOrCreateCorrelationId(context);
    }

    public static void SetCorrelationId(this HttpContext context, string correlationId)
    {
        CorrelationHelper.SetCorrelationId(context, correlationId);
    }

    public static string GetRequestId(this HttpContext context)
    {
        return CorrelationHelper.GetRequestId(context);
    }

    public static void SetRequestId(this HttpContext context, string requestId)
    {
        CorrelationHelper.SetRequestId(context, requestId);
    }

    public static string GetUserAgent(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Headers.UserAgent.ToString();
    }

    public static string GetClientIpAddress(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

        return ip;
    }

    public static bool IsApiRequest(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
               context.Request.Headers.Accept.Any(h => h?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);
    }

    public static bool IsHealthCheckRequest(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
    }

    public static async Task<T?> ReadJsonAsync<T>(this HttpContext context) where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            if (!context.Request.HasJsonContentType())
                return null;
            return await JsonSerializer.DeserializeAsync<T>(
                context.Request.Body,
                _jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static async Task WriteJsonAsync<T>(this HttpContext context, T value, int statusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = HttpConstants.ContentTypes.Json;
        await context.Response.WriteAsync(JsonSerializer.Serialize(value, _jsonOptions));
    }

    public static bool HasHeader(this HttpContext context, string headerName)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Headers.ContainsKey(headerName);
    }

    public static string? GetHeader(this HttpContext context, string headerName)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Headers.TryGetValue(headerName, out var value) ? value.ToString() : null;
    }

    public static void AddResponseHeader(this HttpContext context, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Response.Headers.TryAdd(name, value);
    }

    public static void SetResponseHeader(this HttpContext context, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Response.Headers[name] = value;
    }

    public static bool IsAuthenticated(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User.Identity?.IsAuthenticated == true;
    }

    public static string? GetUserId(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User.GetUserId();
    }

    public static Guid? GetUserIdAsGuid(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User.GetUserIdAsGuid();
    }

    public static string? GetUserEmail(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User.GetEmail();
    }

    public static string? GetUserName(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.User.GetUserName();
    }

    public static IDictionary<string, object> GetRequestContext(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new Dictionary<string, object>
        {
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path.Value ?? string.Empty,
            ["QueryString"] = context.Request.QueryString.Value ?? string.Empty,
            ["UserAgent"] = GetUserAgent(context),
            ["ClientIp"] = GetClientIpAddress(context),
            ["CorrelationId"] = GetCorrelationId(context),
            ["RequestId"] = GetRequestId(context),
            ["TraceId"] = context.TraceIdentifier,
            ["IsAuthenticated"] = IsAuthenticated(context),
            ["UserId"] = GetUserId(context) ?? "anonymous",
            ["Timestamp"] = DateTime.UtcNow
        };
    }

    public static bool IsSecureConnection(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.IsHttps ||
               context.Request.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
    }

    public static Uri GetBaseUrl(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var request = context.Request;
        var scheme = IsSecureConnection(context) ? "https" : "http";
        return new Uri($"{scheme}://{request.Host}{request.PathBase}");
    }

    public static Uri GetFullUrl(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var request = context.Request;
        var scheme = IsSecureConnection(context) ? "https" : "http";
        return new Uri($"{scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}");
    }
}