using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.Json;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class ResponseHelper
{
    public static IResult Success<T>(T data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Ok(response);
    }

    public static IResult Success(string? message = null)
    {
        var response = new ApiResponse
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Ok(response);
    }

    public static IResult PagedSuccess<T>(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        var response = new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Data retrieved successfully",
            CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Pagination = new PaginationInfo
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };
        
        return Results.Ok(response);
    }

    public static IResult Error(string message, int statusCode = 400, IDictionary<string, object>? errors = null)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = message,
            CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = errors,
            TraceId = Activity.Current?.TraceId.ToString()
        };
        
        return Results.Json(response, statusCode: statusCode);
    }

    public static IResult ValidationError(IDictionary<string, string[]> validationErrors, string? message = null)
    {
        var response = new ValidationErrorResponse
        {
            Success = false,
            Message = message ?? "Validation failed",
            CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors
        };
        
        return Results.BadRequest(response);
    }

    public static IResult NotFound(string? message = null)
    {
        return Error(message ?? "Resource not found", 404);
    }

    public static IResult Unauthorized(string? message = null)
    {
        return Error(message ?? "Unauthorized access", 401);
    }

    public static IResult Forbidden(string? message = null)
    {
        return Error(message ?? "Access forbidden", 403);
    }

    public static IResult Conflict(string? message = null)
    {
        return Error(message ?? "Resource conflict", 409);
    }

    public static IResult InternalServerError(string? message = null)
    {
        return Error(message ?? "An internal server error occurred", 500);
    }

    public static async Task<IResult> FromJsonAsync<T>(HttpRequest request, Func<T, Task<IResult>> handler)
    {
        try
        {
            if (!request.HasJsonContentType())
            {
                return Error("Content-Type must be application/json");
            }

            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(json))
            {
                return Error("Request body cannot be empty");
            }

            var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (data == null)
            {
                return Error("Invalid JSON format");
            }

            return await handler(data);
        }
        catch (JsonException ex)
        {
            return Error($"Invalid JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            return InternalServerError($"Request processing error: {ex.Message}");
        }
    }
}