using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Extensions.HttpContext;

public static class RequestExtensions
{
    public static bool HasJsonContentType(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.MediaTypes.Json, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool HasXmlContentType(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.MediaTypes.Xml, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool HasFormContentType(this HttpRequest request)
    {
        return request.ContentType?.Contains(HttpConstants.MediaTypes.FormUrlEncoded, StringComparison.OrdinalIgnoreCase) == true ||
               request.ContentType?.Contains(HttpConstants.MediaTypes.MultipartFormData, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool AcceptsJson(this HttpRequest request)
    {
        return request.Headers.Accept.Any(h => h?.Contains(HttpConstants.MediaTypes.Json, StringComparison.OrdinalIgnoreCase) == true);
    }

    public static bool AcceptsXml(this HttpRequest request)
    {
        return request.Headers.Accept.Any(h => h?.Contains(HttpConstants.MediaTypes.Xml, StringComparison.OrdinalIgnoreCase) == true);
    }

    public static bool AcceptsHtml(this HttpRequest request)
    {
        return request.Headers.Accept.Any(h => h?.Contains(HttpConstants.MediaTypes.TextHtml, StringComparison.OrdinalIgnoreCase) == true);
    }

    public static string GetPreferredAcceptType(this HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept.FirstOrDefault();
        if (string.IsNullOrEmpty(acceptHeader))
            return HttpConstants.MediaTypes.Json;

        // Parse accept header and return the first supported type
        var acceptTypes = acceptHeader.Split(',')
            .Select(h => h.Trim().Split(';')[0])
            .ToArray();

        foreach (var type in acceptTypes)
        {
            if (type.Contains("json", StringComparison.OrdinalIgnoreCase))
                return HttpConstants.MediaTypes.Json;
            if (type.Contains("xml", StringComparison.OrdinalIgnoreCase))
                return HttpConstants.MediaTypes.Xml;
            if (type.Contains("html", StringComparison.OrdinalIgnoreCase))
                return HttpConstants.MediaTypes.TextHtml;
        }

        return HttpConstants.MediaTypes.Json; // Default fallback
    }

    public static async Task<T?> ReadJsonAsync<T>(this HttpRequest request, JsonSerializerOptions? options = null)
    {
        if (!request.HasJsonContentType())
            return default;

        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<string> ReadBodyAsStringAsync(this HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    public static string GetHeaderValue(this HttpRequest request, string headerName)
    {
        return request.Headers[headerName].FirstOrDefault() ?? string.Empty;
    }

    public static string[] GetHeaderValues(this HttpRequest request, string headerName)
    {
        return request.Headers[headerName].ToArray();
    }

    public static bool HasHeader(this HttpRequest request, string headerName)
    {
        return request.Headers.ContainsKey(headerName);
    }

    public static string GetQueryValue(this HttpRequest request, string key)
    {
        return request.Query[key].FirstOrDefault() ?? string.Empty;
    }

    public static string[] GetQueryValues(this HttpRequest request, string key)
    {
        return request.Query[key].ToArray();
    }

    public static bool HasQuery(this HttpRequest request, string key)
    {
        return request.Query.ContainsKey(key);
    }

    public static T? GetQueryValue<T>(this HttpRequest request, string key)
    {
        var value = request.GetQueryValue(key);
        if (string.IsNullOrEmpty(value))
            return default;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    public static int GetPageNumber(this HttpRequest request, string parameterName = "page")
    {
        return request.GetQueryValue<int>(parameterName) is int page && page > 0 ? page : 1;
    }

    public static int GetPageSize(this HttpRequest request, string parameterName = "pageSize", int defaultSize = 20, int maxSize = 100)
    {
        var size = request.GetQueryValue<int>(parameterName);
        if (size <= 0)
            return defaultSize;
        return Math.Min(size, maxSize);
    }

    public static string GetSortBy(this HttpRequest request, string parameterName = "sortBy")
    {
        return request.GetQueryValue(parameterName);
    }

    public static string GetSortDirection(this HttpRequest request, string parameterName = "sortDir")
    {
        var direction = request.GetQueryValue(parameterName).ToLowerInvariant();
        return direction == "desc" || direction == "descending" ? "desc" : "asc";
    }

    public static string GetSearchTerm(this HttpRequest request, string parameterName = "search")
    {
        return request.GetQueryValue(parameterName);
    }

    public static Dictionary<string, string> GetFilters(this HttpRequest request, string prefix = "filter.")
    {
        return request.Query
            .Where(q => q.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                q => q.Key[prefix.Length..],
                q => q.Value.FirstOrDefault() ?? string.Empty
            );
    }

    public static string GetApiKey(this HttpRequest request, string headerName = "X-API-Key")
    {
        // Check header first
        var apiKey = request.GetHeaderValue(headerName);
        if (!string.IsNullOrEmpty(apiKey))
            return apiKey;

        // Check query parameter
        return request.GetQueryValue("apiKey");
    }

    public static string GetBearerToken(this HttpRequest request)
    {
        var authorization = request.GetHeaderValue(HeaderConstants.Authorization);
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization[7..]; // Remove "Bearer " prefix
        }
        return string.Empty;
    }

    public static bool IsMultipart(this HttpRequest request)
    {
        return request.ContentType?.StartsWith(HttpConstants.MediaTypes.MultipartFormData, StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool IsWebSocketRequest(this HttpRequest request)
    {
        return request.Headers.ContainsKey("Upgrade") &&
               request.Headers["Upgrade"].FirstOrDefault()?.Equals("websocket", StringComparison.OrdinalIgnoreCase) == true;
    }

    public static bool IsLocalRequest(this HttpRequest request)
    {
        var connection = request.HttpContext.Connection;
        if (connection.RemoteIpAddress != null)
        {
            return connection.LocalIpAddress != null
                ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                : System.Net.IPAddress.IsLoopback(connection.RemoteIpAddress);
        }
        return true;
    }

    public static string GetClientFingerprint(this HttpRequest request)
    {
        var fingerprint = new
        {
            UserAgent = request.GetHeaderValue(HeaderConstants.UserAgent),
            AcceptLanguage = request.GetHeaderValue(HeaderConstants.AcceptLanguage),
            AcceptEncoding = request.GetHeaderValue(HeaderConstants.AcceptEncoding),
            Accept = request.GetHeaderValue(HeaderConstants.Accept)
        };

        var json = JsonSerializer.Serialize(fingerprint);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }
}