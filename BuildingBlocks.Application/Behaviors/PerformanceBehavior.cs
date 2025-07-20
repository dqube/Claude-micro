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
        var stopwatch = Stopwatch.StartNew();
        
        var response = await next();
        
        stopwatch.Stop();
        
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        var requestName = typeof(TRequest).Name;
        
        if (elapsedMilliseconds > _performanceThresholdMs)
        {
            _logger.LogWarning(
                "Long running request: {RequestName} took {ElapsedMilliseconds}ms (threshold: {ThresholdMs}ms)",
                requestName,
                elapsedMilliseconds,
                _performanceThresholdMs);
        }
        else
        {
            _logger.LogInformation(
                "Request {RequestName} completed in {ElapsedMilliseconds}ms",
                requestName,
                elapsedMilliseconds);
        }
        
        return response;
    }
}