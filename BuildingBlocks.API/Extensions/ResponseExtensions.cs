using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Constants;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Extensions;

public static class ResponseExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    public static void SetContentType(this HttpResponse response, string contentType)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.ContentType = contentType;
    }

    public static void SetJsonContentType(this HttpResponse response)
    {
        response.SetContentType(HttpConstants.ContentTypes.Json);
    }

    public static void SetXmlContentType(this HttpResponse response)
    {
        response.SetContentType(HttpConstants.ContentTypes.Xml);
    }

    public static void SetPlainTextContentType(this HttpResponse response)
    {
        response.SetContentType(HttpConstants.ContentTypes.TextPlain);
    }

    public static void AddHeader(this HttpResponse response, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.Headers.TryAdd(name, value);
    }

    public static void SetHeader(this HttpResponse response, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.Headers[name] = value;
    }

    public static void AddCorrelationId(this HttpResponse response, string correlationId)
    {
        response.AddHeader(HeaderConstants.CorrelationId, correlationId);
    }

    public static void AddRequestId(this HttpResponse response, string requestId)
    {
        response.AddHeader(HeaderConstants.RequestId, requestId);
    }

    public static void AddCacheHeaders(this HttpResponse response, TimeSpan maxAge, bool isPublic = false)
    {
        var cacheControl = isPublic ? "public" : "private";
        response.AddHeader("Cache-Control", $"{cacheControl}, max-age={maxAge.TotalSeconds}");
    }

    public static void AddNoCacheHeaders(this HttpResponse response)
    {
        response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
        response.AddHeader("Pragma", "no-cache");
        response.AddHeader("Expires", "0");
    }

    public static void AddETagHeader(this HttpResponse response, string etag)
    {
        response.AddHeader("ETag", etag);
    }

    public static void AddLocationHeader(this HttpResponse response, string location)
    {
        response.AddHeader("Location", location);
    }

    public static async Task WriteJsonAsync<T>(this HttpResponse response, T value, int statusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.StatusCode = statusCode;
        response.SetJsonContentType();
        await response.WriteAsync(JsonSerializer.Serialize(value, _jsonOptions));
    }

    public static async Task WriteApiResponseAsync<T>(this HttpResponse response, T data, string? message = null, string? correlationId = null, int statusCode = 200)
    {
        var apiResponse = ResponseHelper.Success(data, message, correlationId);
        await response.WriteJsonAsync(apiResponse, statusCode);
    }

    public static async Task WriteApiResponseAsync(this HttpResponse response, string? message = null, string? correlationId = null, int statusCode = 200)
    {
        var apiResponse = ResponseHelper.Success(message, correlationId);
        await response.WriteJsonAsync(apiResponse, statusCode);
    }

    public static async Task WriteErrorResponseAsync(this HttpResponse response, string message, string? errorCode = null, string? correlationId = null, int statusCode = 400)
    {
        var errorResponse = ResponseHelper.Error(message, errorCode, correlationId: correlationId);
        await response.WriteJsonAsync(errorResponse, statusCode);
    }

    public static async Task WriteValidationErrorAsync(this HttpResponse response, IDictionary<string, string[]> errors, string? correlationId = null)
    {
        var validationResponse = ResponseHelper.ValidationError("Validation failed", errors, correlationId);
        await response.WriteJsonAsync(validationResponse, HttpConstants.StatusCodes.UnprocessableEntity);
    }

    public static async Task WriteNotFoundAsync(this HttpResponse response, string resource, string? correlationId = null)
    {
        var errorResponse = ResponseHelper.NotFound(resource, correlationId);
        await response.WriteJsonAsync(errorResponse, HttpConstants.StatusCodes.NotFound);
    }

    public static async Task WriteUnauthorizedAsync(this HttpResponse response, string? message = null, string? correlationId = null)
    {
        var errorResponse = ResponseHelper.Unauthorized(message, correlationId);
        await response.WriteJsonAsync(errorResponse, HttpConstants.StatusCodes.Unauthorized);
    }

    public static async Task WriteForbiddenAsync(this HttpResponse response, string? message = null, string? correlationId = null)
    {
        var errorResponse = ResponseHelper.Forbidden(message, correlationId);
        await response.WriteJsonAsync(errorResponse, HttpConstants.StatusCodes.Forbidden);
    }

    public static async Task WriteConflictAsync(this HttpResponse response, string message, string? correlationId = null)
    {
        var errorResponse = ResponseHelper.Error(message, "CONFLICT", correlationId: correlationId);
        await response.WriteJsonAsync(errorResponse, HttpConstants.StatusCodes.Conflict);
    }


    public static async Task WriteInternalServerErrorAsync(this HttpResponse response, string? message = null, string? correlationId = null)
    {
        var errorResponse = ResponseHelper.Error(
            message ?? "An internal server error occurred",
            "INTERNAL_SERVER_ERROR",
            correlationId: correlationId);
        
        await response.WriteJsonAsync(errorResponse, HttpConstants.StatusCodes.InternalServerError);
    }

    public static async Task WritePagedResponseAsync<T>(
        this HttpResponse response,
        IEnumerable<T> data,
        long totalCount,
        int currentPage,
        int pageSize,
        string? message = null,
        string? correlationId = null)
    {
        var pagedResponse = ResponseHelper.PagedSuccess(data, totalCount, currentPage, pageSize, message, correlationId);
        await response.WriteJsonAsync(pagedResponse);
    }

    public static async Task WriteCreatedAsync<T>(this HttpResponse response, T data, string? location = null, string? message = null, string? correlationId = null)
    {
        if (!string.IsNullOrEmpty(location))
        {
            response.AddLocationHeader(location);
        }

        var apiResponse = ResponseHelper.Success(data, message ?? "Resource created successfully", correlationId);
        await response.WriteJsonAsync(apiResponse, HttpConstants.StatusCodes.Created);
    }

    public static async Task WriteNoContentAsync(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.StatusCode = HttpConstants.StatusCodes.NoContent;
        await Task.CompletedTask;
    }

    public static void SetStatusCode(this HttpResponse response, int statusCode)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.StatusCode = statusCode;
    }

    public static void RedirectTo(this HttpResponse response, string url, bool permanent = false)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.StatusCode = permanent ? 301 : 302;
        response.AddLocationHeader(url);
    }

    public static void RedirectTo(this HttpResponse response, Uri url, bool permanent = false)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(url);
        response.StatusCode = permanent ? 301 : 302;
        response.AddLocationHeader(url.ToString());
    }

    public static bool HasStarted(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return response.HasStarted;
    }

    public static void EnsureNotStarted(this HttpResponse response, string operationName)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (response.HasStarted)
        {
            throw new InvalidOperationException($"Cannot perform {operationName} after response has started");
        }
    }

    public static void AddSecurityHeaders(this HttpResponse response)
    {
        response.AddHeader("X-Content-Type-Options", "nosniff");
        response.AddHeader("X-Frame-Options", "DENY");
        response.AddHeader("X-XSS-Protection", "1; mode=block");
        response.AddHeader("Referrer-Policy", "strict-origin-when-cross-origin");
    }

    public static void AddCorsHeaders(this HttpResponse response, string[]? allowedOrigins = null, string[]? allowedMethods = null)
    {
        var origins = allowedOrigins?.Length > 0 ? string.Join(",", allowedOrigins) : "*";
        var methods = allowedMethods?.Length > 0 ? string.Join(",", allowedMethods) : "GET,POST,PUT,DELETE,OPTIONS";

        response.AddHeader("Access-Control-Allow-Origin", origins);
        response.AddHeader("Access-Control-Allow-Methods", methods);
        response.AddHeader("Access-Control-Allow-Headers", "Content-Type,Authorization,X-Correlation-ID,X-Request-ID");
    }

    public static IDictionary<string, string> GetAllHeaders(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
    }
}