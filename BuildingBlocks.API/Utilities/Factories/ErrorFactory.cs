using BuildingBlocks.API.Responses.Base;
using ErrorHandlingResponse = BuildingBlocks.API.Middleware.ErrorHandling.ErrorResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
// using BuildingBlocks.Domain.Exceptions;
// using BuildingBlocks.Application.Exceptions;

namespace BuildingBlocks.API.Utilities.Factories;

public static class ErrorFactory
{
    public static ErrorResponse CreateFromException(Exception exception, string? correlationId = null)
    {
        var errorResponse = new ErrorResponse
        {
            Success = false,
            Message = GetMessageFromException(exception),
            CorrelationId = correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString(),
            TraceId = Activity.Current?.TraceId.ToString(),
            Timestamp = DateTime.UtcNow
        };

        AddExceptionSpecificData(errorResponse, exception);
        return errorResponse;
    }

    public static ProblemDetails CreateProblemDetailsFromException(Exception exception, HttpContext? context = null)
    {
        var statusCode = GetStatusCodeFromException(exception);
        var title = GetTitleFromException(exception);
        
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context?.Request.Path,
            Type = GetTypeFromStatusCode(statusCode)
        };

        // Add tracing information
        var activity = Activity.Current;
        if (activity != null)
        {
            problemDetails.Extensions["traceId"] = activity.Id;
            problemDetails.Extensions["spanId"] = activity.SpanId.ToString();
        }

        // Add correlation ID
        var correlationId = context?.Items["CorrelationId"]?.ToString() ?? activity?.Id ?? Guid.NewGuid().ToString();
        problemDetails.Extensions["correlationId"] = correlationId;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        // Add exception-specific data
        AddExceptionDataToProblemDetails(problemDetails, exception);

        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        IDictionary<string, string[]> errors,
        HttpContext? context = null,
        string? title = null)
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = 400,
            Title = title ?? "One or more validation errors occurred.",
            Instance = context?.Request.Path,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        // Add tracing information
        var activity = Activity.Current;
        if (activity != null)
        {
            problemDetails.Extensions["traceId"] = activity.Id;
        }

        // Add correlation ID
        var correlationId = context?.Items["CorrelationId"]?.ToString() ?? activity?.Id ?? Guid.NewGuid().ToString();
        problemDetails.Extensions["correlationId"] = correlationId;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
    }

    public static IResult CreateErrorResult(Exception exception, HttpContext? context = null)
    {
        var statusCode = GetStatusCodeFromException(exception);
        var errorResponse = CreateFromException(exception, context?.Items["CorrelationId"]?.ToString());
        
        return Results.Json(errorResponse, statusCode: statusCode);
    }

    public static IResult CreateProblemResult(Exception exception, HttpContext? context = null)
    {
        var problemDetails = CreateProblemDetailsFromException(exception, context);
        return Results.Problem(problemDetails);
    }

    private static string GetMessageFromException(Exception exception)
    {
        return exception switch
        {
            // ValidationException => "Validation failed",
            // DomainException => exception.Message,
            // NotFoundException => "Resource not found",
            ArgumentException => "Invalid request",
            ArgumentNullException => "Missing required parameter",
            UnauthorizedAccessException => "Unauthorized access",
            NotImplementedException => "Feature not implemented",
            _ => "An error occurred while processing your request"
        };
    }

    private static int GetStatusCodeFromException(Exception exception)
    {
        return exception switch
        {
            // ValidationException => 400,
            // DomainException => 400,
            // NotFoundException => 404,
            ArgumentException => 400,
            ArgumentNullException => 400,
            UnauthorizedAccessException => 401,
            NotImplementedException => 501,
            TimeoutException => 408,
            InvalidOperationException => 409,
            _ => 500
        };
    }

    private static string GetTitleFromException(Exception exception)
    {
        return exception switch
        {
            // ValidationException => "Validation Error",
            // DomainException => "Business Rule Violation",
            // NotFoundException => "Resource Not Found",
            ArgumentException => "Invalid Argument",
            ArgumentNullException => "Missing Argument",
            UnauthorizedAccessException => "Unauthorized",
            NotImplementedException => "Not Implemented",
            TimeoutException => "Request Timeout",
            InvalidOperationException => "Invalid Operation",
            _ => "Internal Server Error"
        };
    }

    private static string GetTypeFromStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            408 => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            501 => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }

    private static void AddExceptionSpecificData(ErrorResponse errorResponse, Exception exception)
    {
        errorResponse.Errors = new Dictionary<string, object>();
        errorResponse.Errors["type"] = exception.GetType().Name;

        // Exception-specific handling would go here
        // switch (exception)
        // {
        //     case ValidationException validationEx:
        //         if (validationEx.Errors.Any())
        //         {
        //             errorResponse.Errors["validationErrors"] = validationEx.Errors
        //                 .GroupBy(e => e.PropertyName)
        //                 .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        //         }
        //         break;
        //         
        //     case DomainException domainEx:
        //         if (!string.IsNullOrEmpty(domainEx.ErrorCode))
        //         {
        //             errorResponse.Errors["errorCode"] = domainEx.ErrorCode;
        //         }
        //         break;
        // }
    }

    private static void AddExceptionDataToProblemDetails(ProblemDetails problemDetails, Exception exception)
    {
        // Exception-specific handling would go here
        // switch (exception)
        // {
        //     case ValidationException validationEx when validationEx.Errors.Any():
        //         problemDetails.Extensions["errors"] = validationEx.Errors
        //             .GroupBy(e => e.PropertyName)
        //             .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        //         break;
        //         
        //     case DomainException domainEx:
        //         if (!string.IsNullOrEmpty(domainEx.ErrorCode))
        //         {
        //             problemDetails.Extensions["errorCode"] = domainEx.ErrorCode;
        //         }
        //         break;
        // }

        problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
    }
}