using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly CacheConfiguration _configuration;

    public MemoryCacheService(
        IMemoryCache memoryCache,
        ILogger<MemoryCacheService> logger,
        CacheConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _configuration = configuration;
    }

    public Task<T?> GetAsync<T>(ICacheKey key, CancellationToken cancellationToken = default) where T : class
    {
        return GetAsync<T>(key.Key, cancellationToken);
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var value = _memoryCache.Get<T>(fullKey);
            
            _logger.LogDebug("Cache {Operation} for key: {Key}", value != null ? "Hit" : "Miss", fullKey);
            
            return Task.FromResult(value);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
    }

    public Task SetAsync<T>(ICacheKey key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        var actualPolicy = policy ?? new CachePolicy
        {
            AbsoluteExpiration = key.Expiration,
            SlidingExpiration = null
        };
        return SetAsyncInternal(key.Key, value, actualPolicy, cancellationToken);
    }

    public Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        return SetAsyncInternal(key, value, policy ?? new CachePolicy(), cancellationToken);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        var policy = new CachePolicy { AbsoluteExpiration = expiration };
        return SetAsyncInternal(key, value, policy, cancellationToken);
    }

    private Task SetAsyncInternal<T>(string key, T value, CachePolicy policy, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var options = new MemoryCacheEntryOptions();

            if (policy.AbsoluteExpiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = policy.AbsoluteExpiration.Value;
            }

            if (policy.SlidingExpiration.HasValue)
            {
                options.SlidingExpiration = policy.SlidingExpiration.Value;
            }

            _memoryCache.Set(fullKey, value, options);
            
            _logger.LogDebug("Cache set for key: {Key}", fullKey);
            
            return Task.CompletedTask;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        return RemoveAsync(key.Key, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = GetFullKey(key);
            _memoryCache.Remove(fullKey);
            
            _logger.LogDebug("Cache removed for key: {Key}", fullKey);
            
            return Task.CompletedTask;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Memory cache doesn't support pattern-based removal efficiently
        // This would require keeping track of keys, which is beyond basic implementation
        _logger.LogWarning("Pattern-based cache removal not efficiently supported in MemoryCache");
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Memory cache doesn't have a clear method
        // Would need to track keys or dispose and recreate cache
        _logger.LogWarning("Cache clear not efficiently supported in MemoryCache");
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        return ExistsAsync(key.Key, cancellationToken);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var exists = _memoryCache.TryGetValue(fullKey, out _);
            return Task.FromResult(exists);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return Task.FromResult(false);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public Task RefreshAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        return RefreshAsync(key.Key, cancellationToken);
    }

    public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        // Memory cache doesn't support explicit refresh
        // The value would need to be retrieved and reset
        _logger.LogDebug("Cache refresh requested for key: {Key} (no-op in MemoryCache)", key);
        return Task.CompletedTask;
    }

    private string GetFullKey(string key)
    {
        return $"{_configuration.KeyPrefix}{key}";
    }
}