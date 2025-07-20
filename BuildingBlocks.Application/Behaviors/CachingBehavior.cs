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
        var cacheKey = request.GetCacheKey();
        var cachePolicy = request.GetCachePolicy();

        _logger.LogDebug("Checking cache for key: {CacheKey}", cacheKey.Key);

        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey.Key);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey.Key);

        var response = await next();

        if (response != null)
        {
            await _cacheService.SetAsync(cacheKey, response, cachePolicy, cancellationToken);
            _logger.LogDebug("Cached response for key: {CacheKey}", cacheKey.Key);
        }

        return response!;
    }
}

public interface ICacheableRequest<TResponse>
{
    ICacheKey GetCacheKey();
    CachePolicy GetCachePolicy();
}