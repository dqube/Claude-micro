using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BuildingBlocks.API.Utilities.Constants;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Extensions;

public static class RequestExtensions
{
    public static bool IsJson(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.ContentTypes.Json, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool IsXml(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.ContentTypes.Xml, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool IsFormData(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.ContentTypes.FormUrlEncoded, StringComparison.OrdinalIgnoreCase) == true ||
               request.ContentType?.Contains(HttpConstants.ContentTypes.MultipartFormData, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool IsMultipart(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.ContentTypes.MultipartFormData, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool HasJsonContentType(this HttpRequest request)
    {
        return request.IsJson();
    }

    public static bool IsGetRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Get, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPostRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Post, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPutRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Put, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsDeleteRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Delete, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPatchRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Patch, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsOptionsRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Options, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsHeadRequest(this HttpRequest request)
    {
        return request.Method.Equals(HttpConstants.Methods.Head, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsApiRequest(this HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api") ||
               request.Headers.Accept.Any(h => h?.Contains("application/json") == true);
    }

    public static bool IsHealthCheckRequest(this HttpRequest request)
    {
        return request.Path.StartsWithSegments("/health");
    }

    public static bool IsSwaggerRequest(this HttpRequest request)
    {
        return request.Path.StartsWithSegments("/swagger") ||
               request.Path.StartsWithSegments("/scalar");
    }

    public static string GetUserAgent(this HttpRequest request)
    {
        return request.Headers.UserAgent.ToString();
    }

    public static string GetClientIpAddress(this HttpRequest request)
    {
        var ip = request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                ?? request.Headers["X-Real-IP"].FirstOrDefault()
                ?? request.HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

        return ip;
    }

    public static string? GetHeader(this HttpRequest request, string headerName)
    {
        return request.Headers.TryGetValue(headerName, out var value) ? value.ToString() : null;
    }

    public static bool HasHeader(this HttpRequest request, string headerName)
    {
        return request.Headers.ContainsKey(headerName);
    }

    public static string GetCorrelationId(this HttpRequest request)
    {
        return request.GetHeader(HeaderConstants.CorrelationId) ?? request.HttpContext.TraceIdentifier;
    }

    public static string GetRequestId(this HttpRequest request)
    {
        return request.GetHeader(HeaderConstants.RequestId) ?? request.HttpContext.TraceIdentifier;
    }

    public static string GetApiKey(this HttpRequest request)
    {
        return request.GetHeader(HeaderConstants.ApiKey) ?? string.Empty;
    }

    public static bool HasApiKey(this HttpRequest request)
    {
        return request.HasHeader(HeaderConstants.ApiKey) && !string.IsNullOrEmpty(request.GetApiKey());
    }

    public static bool IsSecure(this HttpRequest request)
    {
        return request.IsHttps ||
               request.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetBaseUrl(this HttpRequest request)
    {
        var scheme = request.IsSecure() ? "https" : "http";
        return $"{scheme}://{request.Host}{request.PathBase}";
    }

    public static string GetFullUrl(this HttpRequest request)
    {
        var scheme = request.IsSecure() ? "https" : "http";
        return $"{scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
    }

    public static async Task<T?> ReadJsonAsync<T>(this HttpRequest request) where T : class
    {
        if (!request.HasJsonContentType())
            return null;

        try
        {
            request.Body.Position = 0;
            return await JsonSerializer.DeserializeAsync<T>(
                request.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
        catch
        {
            return null;
        }
    }

    public static async Task<string> ReadAsStringAsync(this HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync();
    }

    public static T? GetQueryParameter<T>(this HttpRequest request, string parameterName)
    {
        if (!request.Query.TryGetValue(parameterName, out var value))
            return default;

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return default;

        try
        {
            return (T)Convert.ChangeType(stringValue, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    public static string? GetQueryParameter(this HttpRequest request, string parameterName)
    {
        return request.Query.TryGetValue(parameterName, out var value) ? value.ToString() : null;
    }

    public static bool HasQueryParameter(this HttpRequest request, string parameterName)
    {
        return request.Query.ContainsKey(parameterName);
    }

    public static int GetPageNumber(this HttpRequest request, int defaultValue = 1)
    {
        var pageNumber = request.GetQueryParameter<int?>("page") ?? request.GetQueryParameter<int?>("pageNumber") ?? defaultValue;
        return Math.Max(1, pageNumber);
    }

    public static int GetPageSize(this HttpRequest request, int defaultValue = ApiConstants.DefaultPageSize.Medium, int maxValue = ApiConstants.DefaultPageSize.Maximum)
    {
        var pageSize = request.GetQueryParameter<int?>("pageSize") ?? request.GetQueryParameter<int?>("limit") ?? defaultValue;
        return Math.Min(Math.Max(1, pageSize), maxValue);
    }

    public static IDictionary<string, string> GetAllHeaders(this HttpRequest request)
    {
        return request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
    }

    public static IDictionary<string, string> GetAllQueryParameters(this HttpRequest request)
    {
        return request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
    }
}