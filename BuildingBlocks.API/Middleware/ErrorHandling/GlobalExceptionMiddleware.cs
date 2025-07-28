using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BuildingBlocks.API.Responses.Base;
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
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Success = false,
            CorrelationId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ArgumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = exception.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                break;

            case NotImplementedException:
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
                errorResponse.Message = "Feature not implemented";
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "Request timeout";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}