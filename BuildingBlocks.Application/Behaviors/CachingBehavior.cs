using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICacheableRequest<TResponse>
    where TResponse : class
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        var cacheKey = request.GetCacheKey();
        var cachePolicy = request.GetCachePolicy();

        LogCheckingCache(_logger, cacheKey.Key, null);

        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            LogCacheHit(_logger, cacheKey.Key, null);
            return cachedResponse;
        }

        LogCacheMiss(_logger, cacheKey.Key, null);

        var response = await next();

        if (response != null)
        {
            await _cacheService.SetAsync(cacheKey, response, cachePolicy, cancellationToken);
            LogCacheSet(_logger, cacheKey.Key, null);
        }

        return response!;
    }

    private static readonly Action<ILogger, string, Exception?> LogCheckingCache =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "CheckingCache"), "Checking cache for key: {CacheKey}");

    private static readonly Action<ILogger, string, Exception?> LogCacheHit =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2, "CacheHit"), "Cache hit for key: {CacheKey}");

    private static readonly Action<ILogger, string, Exception?> LogCacheMiss =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(3, "CacheMiss"), "Cache miss for key: {CacheKey}");

    private static readonly Action<ILogger, string, Exception?> LogCacheSet =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(4, "CacheSet"), "Cached response for key: {CacheKey}");
}

public interface ICacheableRequest<TResponse>
{
    ICacheKey GetCacheKey();
    CachePolicy GetCachePolicy();
}