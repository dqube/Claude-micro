using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.API.Utilities.Helpers;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Extensions.HttpContext;

public static class HttpContextExtensions
{
    public static string GetCorrelationId(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return CorrelationHelper.GetOrCreateCorrelationId(context);
    }

    public static void SetCorrelationId(this Microsoft.AspNetCore.Http.HttpContext context, string correlationId)
    {
        CorrelationHelper.SetCorrelationId(context, correlationId);
    }

    public static string GetClientIpAddress(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        // Check for forwarded IP headers first
        if (context.Request.Headers.ContainsKey(HeaderConstants.XForwardedFor))
        {
            var forwardedFor = context.Request.Headers[HeaderConstants.XForwardedFor].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }
        }

        if (context.Request.Headers.ContainsKey(HeaderConstants.XRealIp))
        {
            var realIp = context.Request.Headers[HeaderConstants.XRealIp].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }
        }

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    public static string GetUserAgent(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown";
    }

    public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    public static bool IsApiRequest(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/api") ||
               context.Request.Headers.Accept.Any(h => h?.Contains("application/json") == true);
    }

    public static bool HasJsonContentType(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.HasJsonContentType();
    }

    public static async Task<T?> ReadJsonBodyAsync<T>(this Microsoft.AspNetCore.Http.HttpContext context, JsonSerializerOptions? options = null)
    {
        if (!context.Request.HasJsonContentType())
        {
            return default;
        }

        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();
        
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task WriteJsonResponseAsync<T>(this Microsoft.AspNetCore.Http.HttpContext context, T data, int statusCode = 200, JsonSerializerOptions? options = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        var json = JsonSerializer.Serialize(data, options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await context.Response.WriteAsync(json);
    }

    public static string GetRequestId(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.TraceIdentifier;
    }

    public static string GetScheme(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Scheme;
    }

    public static string GetHost(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Host.Value;
    }

    public static string GetBaseUrl(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return $"{context.GetScheme()}://{context.GetHost()}";
    }

    public static string GetFullUrl(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return $"{context.GetBaseUrl()}{context.Request.Path}{context.Request.QueryString}";
    }

    public static Dictionary<string, object> GetRequestMetadata(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return new Dictionary<string, object>
        {
            ["requestId"] = context.GetRequestId(),
            ["correlationId"] = context.GetCorrelationId(),
            ["method"] = context.Request.Method,
            ["path"] = context.Request.Path.Value ?? "",
            ["queryString"] = context.Request.QueryString.Value ?? "",
            ["userAgent"] = context.GetUserAgent(),
            ["clientIp"] = context.GetClientIpAddress(),
            ["scheme"] = context.GetScheme(),
            ["host"] = context.GetHost(),
            ["timestamp"] = DateTime.UtcNow
        };
    }

    public static bool IsHealthCheck(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/health");
    }

    public static bool IsMetricsRequest(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/metrics");
    }

    public static bool IsSwaggerRequest(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/swagger") ||
               context.Request.Path.StartsWithSegments("/scalar");
    }

    public static T? GetService<T>(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        return context.RequestServices.GetService<T>();
    }

    public static T GetRequiredService<T>(this Microsoft.AspNetCore.Http.HttpContext context) where T : notnull
    {
        return context.RequestServices.GetRequiredService<T>();
    }

    public static void SetResponseHeader(this Microsoft.AspNetCore.Http.HttpContext context, string name, string value)
    {
        if (!context.Response.Headers.ContainsKey(name))
        {
            context.Response.Headers.Add(name, value);
        }
    }

    public static void SetCacheHeaders(this Microsoft.AspNetCore.Http.HttpContext context, int maxAgeSeconds)
    {
        context.Response.Headers.CacheControl = $"public, max-age={maxAgeSeconds}";
    }

    public static void SetNoCacheHeaders(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
    }
}