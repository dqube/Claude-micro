using System.Globalization;
using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Responses.Builders;

public static class ErrorResponseBuilder
{
    public static ErrorResponse Build(
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
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ValidationErrorResponse BuildValidation(
        string message,
        IDictionary<string, string[]> validationErrors,
        string? correlationId = null)
    {
        return new ValidationErrorResponse
        {
            Success = false,
            Message = message,
            Errors = validationErrors,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponse NotFound(string resource, string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(resource);
        
        return Build(
            $"{resource} not found",
            "NOT_FOUND",
            $"The requested {resource.ToLower(CultureInfo.InvariantCulture)} could not be found",
            correlationId: correlationId);
    }

    public static ErrorResponse Unauthorized(string? message = null, string? correlationId = null)
    {
        return Build(
            message ?? "Unauthorized access",
            "UNAUTHORIZED",
            "Authentication is required to access this resource",
            correlationId: correlationId);
    }

    public static ErrorResponse Forbidden(string? message = null, string? correlationId = null)
    {
        return Build(
            message ?? "Access forbidden",
            "FORBIDDEN",
            "You don't have permission to access this resource",
            correlationId: correlationId);
    }

    public static ErrorResponse BadRequest(string? message = null, string? correlationId = null)
    {
        return Build(
            message ?? "Bad request",
            "BAD_REQUEST",
            "The request is invalid or malformed",
            correlationId: correlationId);
    }

    public static ErrorResponse Conflict(string? message = null, string? correlationId = null)
    {
        return Build(
            message ?? "Conflict",
            "CONFLICT",
            "The request conflicts with the current state of the resource",
            correlationId: correlationId);
    }

    public static ErrorResponse InternalServerError(string? message = null, string? correlationId = null)
    {
        return Build(
            message ?? "Internal server error",
            "INTERNAL_SERVER_ERROR",
            "An unexpected error occurred while processing the request",
            correlationId: correlationId);
    }
}