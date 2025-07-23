using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRetryableRequest
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        var retryPolicy = request.GetRetryPolicy();
        var maxAttempts = retryPolicy.MaxAttempts;
        var baseDelay = retryPolicy.BaseDelay;

        Exception? lastException = null;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                LogRequestAttempt(_logger, attempt, maxAttempts, null);
                return await next();
            }
            catch (Exception ex) when (attempt < maxAttempts && ShouldRetry(ex, retryPolicy))
            {
                lastException = ex;
                var delay = CalculateDelay(attempt, baseDelay, retryPolicy.BackoffStrategy);
                LogRequestRetry(_logger, attempt, maxAttempts, delay.TotalMilliseconds, ex);
                await Task.Delay(delay, cancellationToken);
            }
        }

        LogRequestFailed(_logger, maxAttempts, lastException);
        throw new InvalidOperationException($"Request failed after {maxAttempts} attempts", lastException);
    }

    private static bool ShouldRetry(Exception exception, RetryPolicy retryPolicy)
    {
        if (retryPolicy.RetryableExceptions?.Length > 0)
        {
            return Array.Exists(retryPolicy.RetryableExceptions, type => type.IsAssignableFrom(exception.GetType()));
        }

        // Default: retry on transient exceptions
        return exception is TimeoutException ||
               exception is HttpRequestException ||
               (exception is TaskCanceledException && !exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase));
    }

    private static TimeSpan CalculateDelay(int attempt, TimeSpan baseDelay, BackoffStrategy strategy)
    {
        return strategy switch
        {
            BackoffStrategy.Fixed => baseDelay,
            BackoffStrategy.Linear => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * attempt),
            BackoffStrategy.Exponential => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1)),
            _ => baseDelay
        };
    }
    private static readonly Action<ILogger, int, int, Exception?> LogRequestAttempt =
        LoggerMessage.Define<int, int>(LogLevel.Debug, new EventId(1, "RequestAttempt"), "Executing request attempt {Attempt}/{MaxAttempts}");

    private static readonly Action<ILogger, int, int, double, Exception?> LogRequestRetry =
        LoggerMessage.Define<int, int, double>(LogLevel.Warning, new EventId(2, "RequestRetry"), "Request failed on attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}ms");

    private static readonly Action<ILogger, int, Exception?> LogRequestFailed =
        LoggerMessage.Define<int>(LogLevel.Error, new EventId(3, "RequestFailed"), "Request failed after {MaxAttempts} attempts");
}

public interface IRetryableRequest
{
    RetryPolicy GetRetryPolicy();
}

public class RetryPolicy
{
    public int MaxAttempts { get; set; } = 3;
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    public BackoffStrategy BackoffStrategy { get; set; } = BackoffStrategy.Exponential;
    public Type[]? RetryableExceptions { get; set; }
}

public enum BackoffStrategy
{
    Fixed,
    Linear,
    Exponential
}