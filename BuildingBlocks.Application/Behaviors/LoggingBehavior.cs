using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private static readonly Action<ILogger, string, DateTime, Exception?> LogHandlingStart =
        LoggerMessage.Define<string, DateTime>(LogLevel.Information, new EventId(1, "HandlingStart"), "Handling {RequestName} at {StartTime}");

    private static readonly Action<ILogger, string, DateTime, Exception?> LogHandlingEnd =
        LoggerMessage.Define<string, DateTime>(LogLevel.Information, new EventId(2, "HandlingEnd"), "Handled {RequestName} at {EndTime}");

    private static readonly Action<ILogger, string, DateTime, Exception?> LogHandlingError =
        LoggerMessage.Define<string, DateTime>(LogLevel.Error, new EventId(3, "HandlingError"), "Error handling {RequestName} at {ErrorTime}");
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        LogHandlingStart(_logger, requestName, DateTime.UtcNow, null);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            LogHandlingEnd(_logger, requestName, DateTime.UtcNow, null);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogHandlingError(_logger, requestName, DateTime.UtcNow, ex);
            
            throw;
        }
    }
}