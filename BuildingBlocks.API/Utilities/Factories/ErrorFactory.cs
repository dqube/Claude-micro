using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.API.Utilities.Constants;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Utilities.Factories;

public static class ErrorFactory
{
    public static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string? type = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type ?? GetRfcType(statusCode),
            Instance = instance ?? httpContext.Request.Path
        };

        EnrichProblemDetails(httpContext, problemDetails);
        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        IDictionary<string, string[]> errors,
        string title = "One or more validation errors occurred.",
        string detail = "See the errors property for details.")
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = HttpConstants.StatusCodes.UnprocessableEntity,
            Title = title,
            Detail = detail,
            Type = GetRfcType(HttpConstants.StatusCodes.UnprocessableEntity),
            Instance = httpContext.Request.Path
        };

        EnrichProblemDetails(httpContext, problemDetails);
        return problemDetails;
    }

    public static ProblemDetails CreateBadRequestProblemDetails(
        HttpContext httpContext,
        string detail = "The request is invalid.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.BadRequest,
            "Bad Request",
            detail);
    }

    public static ProblemDetails CreateNotFoundProblemDetails(
        HttpContext httpContext,
        string detail = "The requested resource was not found.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.NotFound,
            "Not Found",
            detail);
    }

    public static ProblemDetails CreateUnauthorizedProblemDetails(
        HttpContext httpContext,
        string detail = "Authentication is required to access this resource.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.Unauthorized,
            "Unauthorized",
            detail);
    }

    public static ProblemDetails CreateForbiddenProblemDetails(
        HttpContext httpContext,
        string detail = "You don't have permission to access this resource.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.Forbidden,
            "Forbidden",
            detail);
    }

    public static ProblemDetails CreateConflictProblemDetails(
        HttpContext httpContext,
        string detail = "The request conflicts with the current state of the resource.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.Conflict,
            "Conflict",
            detail);
    }

    public static ProblemDetails CreateInternalServerErrorProblemDetails(
        HttpContext httpContext,
        string detail = "An internal server error occurred.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.InternalServerError,
            "Internal Server Error",
            detail);
    }

    public static ProblemDetails CreateTooManyRequestsProblemDetails(
        HttpContext httpContext,
        string detail = "Too many requests. Please try again later.")
    {
        return CreateProblemDetails(
            httpContext,
            HttpConstants.StatusCodes.TooManyRequests,
            "Too Many Requests",
            detail);
    }

    private static void EnrichProblemDetails(HttpContext httpContext, ProblemDetails problemDetails)
    {
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["correlationId"] = CorrelationHelper.GetCorrelationId(httpContext);
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        
        if (problemDetails.Status >= 500)
        {
            problemDetails.Extensions["supportReference"] = $"SR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
        }
    }

    private static string GetRfcType(int statusCode) => statusCode switch
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
}