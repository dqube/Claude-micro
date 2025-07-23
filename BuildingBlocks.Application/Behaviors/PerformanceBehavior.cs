using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly long _performanceThresholdMs;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        long performanceThresholdMs = 500)
    {
        _logger = logger;
        _performanceThresholdMs = performanceThresholdMs;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        var requestName = typeof(TRequest).Name;

        if (elapsedMilliseconds > _performanceThresholdMs)
        {
            LogPerformanceWarning(_logger, requestName, elapsedMilliseconds, _performanceThresholdMs, null);
        }
        else
        {
            LogPerformanceInfo(_logger, requestName, elapsedMilliseconds, null);
        }

        return response;
    }

    private static readonly Action<ILogger, string, long, long, Exception?> LogPerformanceWarning =
        LoggerMessage.Define<string, long, long>(LogLevel.Warning, new EventId(1, "PerformanceWarning"), "Long running request: {RequestName} took {ElapsedMilliseconds}ms (threshold: {ThresholdMs}ms)");

    private static readonly Action<ILogger, string, long, Exception?> LogPerformanceInfo =
        LoggerMessage.Define<string, long>(LogLevel.Information, new EventId(2, "PerformanceInfo"), "Request {RequestName} completed in {ElapsedMilliseconds}ms");
}