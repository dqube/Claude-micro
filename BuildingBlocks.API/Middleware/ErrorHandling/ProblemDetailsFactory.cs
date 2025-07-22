using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
// using BuildingBlocks.Domain.Exceptions;
// using BuildingBlocks.Application.Exceptions;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

public static class ProblemDetailsFactory
{
    public static ProblemDetails CreateProblemDetails(
        HttpContext context,
        Exception exception,
        int? statusCode = null,
        string? title = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = CreateProblemDetails(
            context,
            statusCode ?? GetStatusCodeFromException(exception),
            title ?? GetTitleFromException(exception),
            detail ?? exception.Message,
            instance
        );

        // Add exception-specific properties
        AddExceptionDetails(problemDetails, exception);

        return problemDetails;
    }

    public static ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string? title = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title ?? GetDefaultTitle(statusCode),
            Detail = detail,
            Instance = instance ?? context.Request.Path,
            Type = GetTypeFromStatusCode(statusCode)
        };

        // Add tracing information
        var activity = Activity.Current;
        if (activity != null)
        {
            problemDetails.Extensions["traceId"] = activity.Id;
        }

        // Add correlation ID if available
        if (context.Items.ContainsKey("CorrelationId"))
        {
            problemDetails.Extensions["correlationId"] = context.Items["CorrelationId"];
        }

        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        IDictionary<string, string[]> errors,
        string? title = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = 400,
            Title = title ?? "One or more validation errors occurred.",
            Detail = detail,
            Instance = instance ?? context.Request.Path,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        // Add tracing information
        var activity = Activity.Current;
        if (activity != null)
        {
            problemDetails.Extensions["traceId"] = activity.Id;
        }

        // Add correlation ID if available
        if (context.Items.ContainsKey("CorrelationId"))
        {
            problemDetails.Extensions["correlationId"] = context.Items["CorrelationId"];
        }

        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return problemDetails;
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
            _ => 500
        };
    }

    private static string GetTitleFromException(Exception exception)
    {
        return exception switch
        {
            // ValidationException => "Validation Error",
            // DomainException => "Domain Error", 
            // NotFoundException => "Resource Not Found",
            ArgumentException => "Bad Request",
            ArgumentNullException => "Bad Request",
            UnauthorizedAccessException => "Unauthorized",
            NotImplementedException => "Not Implemented",
            _ => "An error occurred"
        };
    }

    private static string GetDefaultTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            422 => "Unprocessable Entity",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            _ => "An error occurred"
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
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            501 => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            502 => "https://tools.ietf.org/html/rfc7231#section-6.6.3",
            503 => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }

    private static void AddExceptionDetails(ProblemDetails problemDetails, Exception exception)
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
        //             problemDetails.Extensions["errorCode"] = domainEx.ErrorCode;
        //         break;
        // }
    }
}