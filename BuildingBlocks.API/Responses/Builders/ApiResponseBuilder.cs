using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Responses.Builders;

public static class ApiResponseBuilder
{
    public static ApiResponse<T> Success<T>(T data, string? message = null, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse Success(string? message = null, string? correlationId = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static PagedResponse<T> PagedSuccess<T>(
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
            Message = message ?? "Retrieved successfully",
            CorrelationId = correlationId,
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

    public static ErrorResponse Error(string message, string? correlationId = null, IDictionary<string, object>? errors = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Errors = errors
        };
    }

    public static ValidationErrorResponse ValidationError(
        IDictionary<string, string[]> validationErrors,
        string? message = null,
        string? correlationId = null)
    {
        return new ValidationErrorResponse
        {
            Success = false,
            Message = message ?? "Validation failed",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors
        };
    }

    public static ErrorResponse NotFound(string? message = null, string? correlationId = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message ?? "Resource not found",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse Unauthorized(string? message = null, string? correlationId = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message ?? "Unauthorized access",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse Forbidden(string? message = null, string? correlationId = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message ?? "Access forbidden",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse InternalServerError(string? message = null, string? correlationId = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message ?? "An internal server error occurred",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }
}