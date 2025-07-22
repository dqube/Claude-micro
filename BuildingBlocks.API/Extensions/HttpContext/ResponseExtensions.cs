using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Extensions.HttpContext;

public static class ResponseExtensions
{
    public static async Task WriteJsonAsync<T>(this HttpResponse response, T data, JsonSerializerOptions? options = null)
    {
        response.ContentType = HttpConstants.MediaTypes.Json;
        
        var json = JsonSerializer.Serialize(data, options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        
        await response.WriteAsync(json);
    }

    public static async Task WriteApiResponseAsync<T>(this HttpResponse response, T data, string? message = null, int statusCode = 200)
    {
        var apiResponse = new ApiResponse<T>
        {
            Success = statusCode < 400,
            Data = data,
            Message = message ?? "Operation completed successfully",
            Timestamp = DateTime.UtcNow
        };

        response.StatusCode = statusCode;
        await response.WriteJsonAsync(apiResponse);
    }

    public static async Task WriteErrorResponseAsync(this HttpResponse response, string message, int statusCode = 400, IDictionary<string, object>? errors = null)
    {
        var errorResponse = new ErrorResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow,
            Errors = errors
        };

        response.StatusCode = statusCode;
        await response.WriteJsonAsync(errorResponse);
    }

    public static async Task WriteValidationErrorResponseAsync(this HttpResponse response, IDictionary<string, string[]> validationErrors, string? message = null)
    {
        var errorResponse = new ValidationErrorResponse
        {
            Success = false,
            Message = message ?? "Validation failed",
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors
        };

        response.StatusCode = 400;
        await response.WriteJsonAsync(errorResponse);
    }

    public static async Task WritePagedResponseAsync<T>(
        this HttpResponse response,
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        var pagedResponse = new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Data retrieved successfully",
            Timestamp = DateTime.UtcNow,
            Pagination = new PaginationInfo
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };

        await response.WriteJsonAsync(pagedResponse);
    }

    public static void SetCorrelationId(this HttpResponse response, string correlationId)
    {
        if (!response.Headers.ContainsKey(HeaderConstants.CorrelationId))
        {
            response.Headers.Add(HeaderConstants.CorrelationId, correlationId);
        }
    }

    public static void SetCacheHeaders(this HttpResponse response, int maxAgeSeconds, bool isPublic = true)
    {
        var cacheControl = isPublic ? "public" : "private";
        response.Headers.CacheControl = $"{cacheControl}, max-age={maxAgeSeconds}";
    }

    public static void SetNoCacheHeaders(this HttpResponse response)
    {
        response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        response.Headers.Pragma = "no-cache";
        response.Headers.Expires = "0";
    }

    public static void SetETagHeader(this HttpResponse response, string etag)
    {
        response.Headers.ETag = $"\"{etag}\"";
    }

    public static void SetLastModifiedHeader(this HttpResponse response, DateTime lastModified)
    {
        response.Headers.LastModified = lastModified.ToString("R");
    }

    public static void SetLocationHeader(this HttpResponse response, string location)
    {
        response.Headers.Location = location;
    }

    public static void SetSecurityHeaders(this HttpResponse response)
    {
        response.Headers.Add("X-Content-Type-Options", "nosniff");
        response.Headers.Add("X-Frame-Options", "DENY");
        response.Headers.Add("X-XSS-Protection", "1; mode=block");
        response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    }

    public static void SetRateLimitHeaders(this HttpResponse response, int limit, int remaining, DateTimeOffset reset)
    {
        response.Headers.Add(HeaderConstants.RateLimitLimit, limit.ToString());
        response.Headers.Add(HeaderConstants.RateLimitRemaining, remaining.ToString());
        response.Headers.Add(HeaderConstants.RateLimitReset, reset.ToUnixTimeSeconds().ToString());
    }

    public static void SetPaginationHeaders(this HttpResponse response, int currentPage, int pageSize, long totalCount)
    {
        response.Headers.Add(HeaderConstants.TotalCount, totalCount.ToString());
        response.Headers.Add(HeaderConstants.CurrentPage, currentPage.ToString());
        response.Headers.Add(HeaderConstants.PageSize, pageSize.ToString());
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        response.Headers.Add(HeaderConstants.PageCount, totalPages.ToString());
    }

    public static void SetCustomHeader(this HttpResponse response, string name, string value)
    {
        if (!response.Headers.ContainsKey(name))
        {
            response.Headers.Add(name, value);
        }
    }

    public static void RemoveHeader(this HttpResponse response, string name)
    {
        if (response.Headers.ContainsKey(name))
        {
            response.Headers.Remove(name);
        }
    }

    public static bool HasStarted(this HttpResponse response)
    {
        return response.HasStarted;
    }

    public static async Task WriteStatusAsync(this HttpResponse response, int statusCode, string? message = null)
    {
        response.StatusCode = statusCode;
        
        if (!string.IsNullOrEmpty(message))
        {
            response.ContentType = HttpConstants.MediaTypes.TextPlain;
            await response.WriteAsync(message);
        }
    }

    public static async Task WriteNotFoundAsync(this HttpResponse response, string? message = null)
    {
        await response.WriteErrorResponseAsync(message ?? "Resource not found", 404);
    }

    public static async Task WriteUnauthorizedAsync(this HttpResponse response, string? message = null)
    {
        await response.WriteErrorResponseAsync(message ?? "Unauthorized access", 401);
    }

    public static async Task WriteForbiddenAsync(this HttpResponse response, string? message = null)
    {
        await response.WriteErrorResponseAsync(message ?? "Access forbidden", 403);
    }

    public static async Task WriteBadRequestAsync(this HttpResponse response, string? message = null)
    {
        await response.WriteErrorResponseAsync(message ?? "Bad request", 400);
    }

    public static async Task WriteInternalServerErrorAsync(this HttpResponse response, string? message = null)
    {
        await response.WriteErrorResponseAsync(message ?? "Internal server error", 500);
    }

    public static async Task WriteNoContentAsync(this HttpResponse response)
    {
        response.StatusCode = 204;
        await response.CompleteAsync();
    }

    public static async Task WriteCreatedAsync<T>(this HttpResponse response, T data, string? location = null, string? message = null)
    {
        if (!string.IsNullOrEmpty(location))
        {
            response.SetLocationHeader(location);
        }
        
        await response.WriteApiResponseAsync(data, message ?? "Resource created successfully", 201);
    }

    public static void SetContentType(this HttpResponse response, string contentType)
    {
        response.ContentType = contentType;
    }

    public static void SetJsonContentType(this HttpResponse response)
    {
        response.ContentType = HttpConstants.MediaTypes.Json;
    }

    public static void SetXmlContentType(this HttpResponse response)
    {
        response.ContentType = HttpConstants.MediaTypes.Xml;
    }

    public static void SetTextContentType(this HttpResponse response)
    {
        response.ContentType = HttpConstants.MediaTypes.TextPlain;
    }
}