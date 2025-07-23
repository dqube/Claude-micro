using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.API.Responses.Base;
using System.Security.Claims;

namespace BuildingBlocks.API.Endpoints.Base;

public abstract class EndpointBase
{
    // Custom mediator interface will be injected when available
    // For now, endpoints can work without mediator or inject services directly
    
    protected static string? GetUserId(ClaimsPrincipal user)
    {
        return user?.Identity?.Name ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
    
    protected static Guid? GetUserIdAsGuid(ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        return Guid.TryParse(userId, out var id) ? id : null;
    }
    
    protected static string GetCorrelationId(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.TraceIdentifier;
    }
    
    protected static IResult ApiResponse<T>(T data, string? message = null, string? correlationId = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Ok(response);
    }
    
    protected static IResult ApiResponse(string? message = null, string? correlationId = null)
    {
        var response = new ApiResponse
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Ok(response);
    }
    
    protected static IResult ApiError(string message, int statusCode = 400, string? correlationId = null)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Problem(
            detail: response.Message,
            statusCode: statusCode,
            title: GetStatusTitle(statusCode),
            extensions: new Dictionary<string, object?>
            {
                ["correlationId"] = response.CorrelationId,
                ["timestamp"] = response.Timestamp,
                ["success"] = response.Success
            });
    }
    
    protected static IResult ValidationError(IDictionary<string, string[]> errors, string? correlationId = null)
    {
        var response = new ValidationErrorResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.ValidationProblem(
            errors,
            detail: response.Message,
            title: "Validation Error",
            extensions: new Dictionary<string, object?>
            {
                ["correlationId"] = response.CorrelationId,
                ["timestamp"] = response.Timestamp,
                ["success"] = response.Success
            });
    }
    
    protected static IResult Created<T>(T data, string? location = null, string? message = null, string? correlationId = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Resource created successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return location != null 
            ? Results.Created(location, response)
            : Results.Created(string.Empty, response);
    }
    
    protected static IResult NoContent(string? message = null, string? correlationId = null)
    {
        return Results.NoContent();
    }
    
    protected static IResult NotFound(string? message = null, string? correlationId = null)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = message ?? "Resource not found",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.NotFound(response);
    }
    
    protected static IResult Unauthorized(string? message = null, string? correlationId = null)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = message ?? "Unauthorized access",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Unauthorized();
    }
    
    protected static IResult Forbidden(string? message = null, string? correlationId = null)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = message ?? "Access forbidden",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        return Results.Problem(
            detail: response.Message,
            statusCode: 403,
            title: "Forbidden");
    }
    
    private static string GetStatusTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        500 => "Internal Server Error",
        _ => "Error"
    };
}