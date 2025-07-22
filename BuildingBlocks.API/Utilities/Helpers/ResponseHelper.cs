using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class ResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string? message = null, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? ApiConstants.DefaultSuccessMessage,
            CorrelationId = correlationId ?? ApiConstants.DefaultCorrelationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse Success(string? message = null, string? correlationId = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message ?? ApiConstants.DefaultSuccessMessage,
            CorrelationId = correlationId ?? ApiConstants.DefaultCorrelationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static PagedResponse<T> PagedSuccess<T>(
        IEnumerable<T> data,
        long totalCount,
        int currentPage,
        int pageSize,
        string? message = null,
        string? correlationId = null)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? ApiConstants.DefaultSuccessMessage,
            CorrelationId = correlationId ?? ApiConstants.DefaultCorrelationId,
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

    public static ErrorResponse Error(
        string message,
        string? errorCode = null,
        string? details = null,
        IDictionary<string, object>? errors = null,
        string? correlationId = null)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Details = details,
            Errors = errors,
            CorrelationId = correlationId ?? ApiConstants.DefaultCorrelationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ValidationErrorResponse ValidationError(
        string message,
        IDictionary<string, string[]> errors,
        string? correlationId = null)
    {
        return new ValidationErrorResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            CorrelationId = correlationId ?? ApiConstants.DefaultCorrelationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse NotFound(string resource, string? correlationId = null)
    {
        return Error(
            $"{resource} not found",
            "NOT_FOUND", 
            $"The requested {resource.ToLower()} could not be found",
            correlationId: correlationId);
    }

    public static ErrorResponse Unauthorized(string? message = null, string? correlationId = null)
    {
        return Error(
            message ?? ApiConstants.StatusMessages.Unauthorized,
            "UNAUTHORIZED",
            "Authentication is required to access this resource",
            correlationId: correlationId);
    }

    public static ErrorResponse Forbidden(string? message = null, string? correlationId = null)
    {
        return Error(
            message ?? ApiConstants.StatusMessages.Forbidden,
            "FORBIDDEN",
            "You don't have permission to access this resource",
            correlationId: correlationId);
    }
}