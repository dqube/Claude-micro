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
        var retryPolicy = request.GetRetryPolicy();
        var maxAttempts = retryPolicy.MaxAttempts;
        var baseDelay = retryPolicy.BaseDelay;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                _logger.LogDebug("Executing request attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                return await next();
            }
            catch (Exception ex) when (attempt < maxAttempts && ShouldRetry(ex, retryPolicy))
            {
                var delay = CalculateDelay(attempt, baseDelay, retryPolicy.BackoffStrategy);
                _logger.LogWarning(ex, "Request failed on attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}ms", 
                    attempt, maxAttempts, delay.TotalMilliseconds);
                
                await Task.Delay(delay, cancellationToken);
            }
        }

        _logger.LogError("Request failed after {MaxAttempts} attempts", maxAttempts);
        return await next(); // Final attempt without retry
    }

    private bool ShouldRetry(Exception exception, RetryPolicy retryPolicy)
    {
        if (retryPolicy.RetryableExceptions?.Any() == true)
        {
            return retryPolicy.RetryableExceptions.Any(type => type.IsAssignableFrom(exception.GetType()));
        }

        // Default: retry on transient exceptions
        return exception is TimeoutException ||
               exception is HttpRequestException ||
               exception is TaskCanceledException && !exception.Message.Contains("timeout");
    }

    private TimeSpan CalculateDelay(int attempt, TimeSpan baseDelay, BackoffStrategy strategy)
    {
        return strategy switch
        {
            BackoffStrategy.Fixed => baseDelay,
            BackoffStrategy.Linear => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * attempt),
            BackoffStrategy.Exponential => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1)),
            _ => baseDelay
        };
    }
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