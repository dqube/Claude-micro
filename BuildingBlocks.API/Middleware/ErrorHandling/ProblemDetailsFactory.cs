using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

public static class ProblemDetailsFactory
{
    public static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        statusCode ??= httpContext.Response.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title ?? GetDefaultTitle(statusCode.Value),
            Type = type ?? GetDefaultType(statusCode.Value),
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        ApplyDefaults(httpContext, problemDetails, statusCode.Value);

        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        IDictionary<string, string[]> errors,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(errors);
        
        statusCode ??= HttpConstants.StatusCodes.UnprocessableEntity;

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = statusCode,
            Title = title ?? "One or more validation errors occurred.",
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        ApplyDefaults(httpContext, problemDetails, statusCode.Value);

        return problemDetails;
    }

    private static void ApplyDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
    {
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        if (httpContext.Request.Headers.TryGetValue(HeaderConstants.CorrelationId, out var correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId.ToString();
        }

        if (statusCode >= 500)
        {
            problemDetails.Extensions["supportReference"] = GenerateSupportReference();
        }
    }

    private static string GetDefaultTitle(int statusCode) => statusCode switch
    {
        HttpConstants.StatusCodes.BadRequest => "Bad Request",
        HttpConstants.StatusCodes.Unauthorized => "Unauthorized",
        HttpConstants.StatusCodes.Forbidden => "Forbidden",
        HttpConstants.StatusCodes.NotFound => "Not Found",
        HttpConstants.StatusCodes.Conflict => "Conflict",
        HttpConstants.StatusCodes.UnprocessableEntity => "Unprocessable Entity",
        HttpConstants.StatusCodes.TooManyRequests => "Too Many Requests",
        HttpConstants.StatusCodes.InternalServerError => "Internal Server Error",
        _ => "An error occurred"
    };

    private static string GetDefaultType(int statusCode) => statusCode switch
    {
        HttpConstants.StatusCodes.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        HttpConstants.StatusCodes.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        HttpConstants.StatusCodes.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        HttpConstants.StatusCodes.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        HttpConstants.StatusCodes.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        HttpConstants.StatusCodes.UnprocessableEntity => "https://tools.ietf.org/html/rfc4918#section-11.2",
        HttpConstants.StatusCodes.TooManyRequests => "https://tools.ietf.org/html/rfc6585#section-4",
        HttpConstants.StatusCodes.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        _ => "about:blank"
    };

    private static string GenerateSupportReference()
    {
        return $"SR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}