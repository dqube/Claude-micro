using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BuildingBlocks.API.Middleware.ErrorHandling;

public class GlobalExceptionMiddleware
{
    private static readonly Action<ILogger, Exception, string, Exception?> _logUnhandledException =
        LoggerMessage.Define<Exception, string>(
            LogLevel.Error,
            new EventId(0, nameof(GlobalExceptionMiddleware)),
            "An unhandled exception occurred. Exception: {Exception}. CorrelationId: {CorrelationId}");

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleExceptionAsync(context, ex);
        }
        catch (NotImplementedException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleExceptionAsync(context, ex);
        }
        catch (TimeoutException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleExceptionAsync(context, ex);
        }
        catch (ValidationException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (FluentValidationException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleFluentValidationExceptionAsync(context, ex);
        }
        catch (BusinessRuleValidationException ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleBusinessRuleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logUnhandledException(_logger, ex, context.TraceIdentifier, ex);
            await HandleExceptionAsync(context, ex);
            throw;
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/problem+json";

        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Bad Request", exception.Message),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized", "Unauthorized access"),
            NotImplementedException => ((int)HttpStatusCode.NotImplemented, "Not Implemented", "Feature not implemented"),
            TimeoutException => ((int)HttpStatusCode.RequestTimeout, "Request Timeout", "Request timeout"),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error", "An internal server error occurred")
        };

        response.StatusCode = statusCode;

        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            context,
            statusCode,
            title,
            detail: detail);

        var jsonResponse = JsonSerializer.Serialize(problemDetails, _jsonOptions);
        await response.WriteAsync(jsonResponse);
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.ContentType = "application/problem+json";

        // Convert validation errors to the format expected by ValidationProblemDetails
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(
            context,
            errors,
            (int)HttpStatusCode.BadRequest,
            "One or more validation errors occurred.",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "The request contained invalid data.");

        var jsonResponse = JsonSerializer.Serialize(problemDetails, _jsonOptions);
        await response.WriteAsync(jsonResponse);
    }

    private static async Task HandleBusinessRuleExceptionAsync(HttpContext context, BusinessRuleValidationException businessRuleException)
    {
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.ContentType = "application/problem+json";

        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            context,
            (int)HttpStatusCode.BadRequest,
            "Business Rule Violation",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            businessRuleException.Message);

        // Add business rule specific information
        if (businessRuleException.BrokenRule != null)
        {
            problemDetails.Extensions["businessRule"] = businessRuleException.BrokenRule.GetType().Name;
            problemDetails.Extensions["ruleMessage"] = businessRuleException.BrokenRule.Message;
        }

        var jsonResponse = JsonSerializer.Serialize(problemDetails, _jsonOptions);
        await response.WriteAsync(jsonResponse);
    }

    private static async Task HandleFluentValidationExceptionAsync(HttpContext context, FluentValidationException fluentValidationException)
    {
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.ContentType = "application/problem+json";

        // Convert FluentValidation errors to the format expected by ValidationProblemDetails
        var errors = fluentValidationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(
            context,
            errors,
            (int)HttpStatusCode.BadRequest,
            "One or more validation errors occurred.",
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "The request contained invalid data.");

        var jsonResponse = JsonSerializer.Serialize(problemDetails, _jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}