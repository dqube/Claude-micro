using BuildingBlocks.API.Responses.Base;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace BuildingBlocks.API.Utilities.Factories;

public static class ResponseFactory
{
    public static ApiResponse<T> CreateSuccess<T>(T data, string? message = null, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse CreateSuccess(string? message = null, string? correlationId = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        };
    }

    public static PagedResponse<T> CreatePagedSuccess<T>(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        long totalCount,
        string? message = null,
        string? correlationId = null)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Data retrieved successfully",
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Pagination = new PaginationInfo
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };
    }

    public static ErrorResponse CreateError(string message, string? correlationId = null, string? traceId = null, string? instance = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            TraceId = traceId ?? Activity.Current?.TraceId.ToString(),
            Instance = instance,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse CreateErrorWithDetails(
        string message,
        IDictionary<string, object> errors,
        string? correlationId = null,
        string? traceId = null,
        string? instance = null)
    {
        var errorResponse = CreateError(message, correlationId, traceId, instance);
        errorResponse.Errors = errors;
        return errorResponse;
    }

    public static ValidationErrorResponse CreateValidationError(
        IDictionary<string, string[]> validationErrors,
        string? message = null,
        string? correlationId = null)
    {
        return new ValidationErrorResponse
        {
            Success = false,
            Message = message ?? "Validation failed",
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors
        };
    }

    public static IResult CreateSuccessResult<T>(T data, string? message = null)
    {
        var response = CreateSuccess(data, message);
        return Results.Ok(response);
    }

    public static IResult CreateSuccessResult(string? message = null)
    {
        var response = CreateSuccess(message);
        return Results.Ok(response);
    }

    public static IResult CreateCreatedResult<T>(T data, string location, string? message = null)
    {
        var response = CreateSuccess(data, message ?? "Resource created successfully");
        return Results.Created(location, response);
    }

    public static IResult CreatePagedResult<T>(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        var response = CreatePagedSuccess(data, currentPage, pageSize, totalCount, message);
        return Results.Ok(response);
    }

    public static IResult CreateErrorResult(string message, int statusCode = 400)
    {
        var response = CreateError(message);
        return Results.Json(response, statusCode: statusCode);
    }

    public static IResult CreateValidationErrorResult(IDictionary<string, string[]> validationErrors, string? message = null)
    {
        var response = CreateValidationError(validationErrors, message);
        return Results.BadRequest(response);
    }

    public static IResult CreateNotFoundResult(string? message = null)
    {
        return CreateErrorResult(message ?? "Resource not found", StatusCodes.Status404NotFound);
    }

    public static IResult CreateUnauthorizedResult(string? message = null)
    {
        return CreateErrorResult(message ?? "Unauthorized access", StatusCodes.Status401Unauthorized);
    }

    public static IResult CreateForbiddenResult(string? message = null)
    {
        return CreateErrorResult(message ?? "Access forbidden", StatusCodes.Status403Forbidden);
    }

    public static IResult CreateConflictResult(string? message = null)
    {
        return CreateErrorResult(message ?? "Resource conflict", StatusCodes.Status409Conflict);
    }

    public static IResult CreateNoContentResult()
    {
        return Results.NoContent();
    }
}