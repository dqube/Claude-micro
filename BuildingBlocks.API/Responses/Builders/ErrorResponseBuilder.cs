using BuildingBlocks.API.Responses.Base;
using System.Diagnostics;

namespace BuildingBlocks.API.Responses.Builders;

public class ErrorResponseBuilder
{
    private readonly ErrorResponse _response;

    public ErrorResponseBuilder()
    {
        _response = new ErrorResponse
        {
            Success = false,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponseBuilder Create() => new();

    public ErrorResponseBuilder WithMessage(string message)
    {
        _response.Message = message;
        return this;
    }

    public ErrorResponseBuilder WithCorrelationId(string correlationId)
    {
        _response.CorrelationId = correlationId;
        return this;
    }

    public ErrorResponseBuilder WithTraceId(string? traceId)
    {
        _response.TraceId = traceId;
        return this;
    }

    public ErrorResponseBuilder WithInstance(string instance)
    {
        _response.Instance = instance;
        return this;
    }

    public ErrorResponseBuilder WithError(string key, object value)
    {
        _response.Errors ??= new Dictionary<string, object>();
        _response.Errors[key] = value;
        return this;
    }

    public ErrorResponseBuilder WithErrors(IDictionary<string, object> errors)
    {
        _response.Errors = errors;
        return this;
    }

    public ErrorResponseBuilder FromException(Exception exception, bool includeStackTrace = false)
    {
        _response.Message = exception.Message;
        _response.Errors ??= new Dictionary<string, object>();
        _response.Errors["type"] = exception.GetType().Name;
        
        if (includeStackTrace && exception.StackTrace != null)
        {
            _response.Errors["stackTrace"] = exception.StackTrace;
        }

        if (exception.InnerException != null)
        {
            _response.Errors["innerException"] = exception.InnerException.Message;
        }

        return this;
    }

    public ErrorResponse Build() => _response;

    public static ErrorResponse BadRequest(string message, string? correlationId = null)
    {
        return Create()
            .WithMessage(message)
            .WithCorrelationId(correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString())
            .Build();
    }

    public static ErrorResponse NotFound(string message, string? correlationId = null)
    {
        return Create()
            .WithMessage(message)
            .WithCorrelationId(correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString())
            .Build();
    }

    public static ErrorResponse InternalServerError(string message = "An internal server error occurred", string? correlationId = null)
    {
        return Create()
            .WithMessage(message)
            .WithCorrelationId(correlationId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString())
            .Build();
    }
}