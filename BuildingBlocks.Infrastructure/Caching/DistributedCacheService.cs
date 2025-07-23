using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Infrastructure.Caching;

public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCacheService> _logger;

    public DistributedCacheService(
        IDistributedCache distributedCache, 
        ILogger<DistributedCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(ICacheKey key, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
        return await GetAsync<T>(key.Key, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(ICacheKey key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        await SetAsync(key.Key, value, policy, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (policy?.AbsoluteExpiration.HasValue == true)
            {
                options.AbsoluteExpirationRelativeToNow = policy.AbsoluteExpiration.Value;
            }

            if (policy?.SlidingExpiration.HasValue == true)
            {
                options.SlidingExpiration = policy.SlidingExpiration.Value;
            }

            await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
            
            _logger.LogDebug("Cache set for key: {Key}", key);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        await RemoveAsync(key.Key, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        // Distributed cache doesn't support pattern-based removal efficiently
        _logger.LogWarning("Pattern-based cache removal not efficiently supported in DistributedCache");
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        return await ExistsAsync(key.Key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        try
        {
            var value = await _distributedCache.GetStringAsync(key, cancellationToken);
            return !string.IsNullOrEmpty(value);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public Task RefreshAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        return RefreshAsync(key.Key, cancellationToken);
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        try
        {
            await _distributedCache.RefreshAsync(key, cancellationToken);
            _logger.LogDebug("Cache refreshed for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Distributed cache doesn't have a clear method
        _logger.LogWarning("Cache clear not efficiently supported in DistributedCache");
        return Task.CompletedTask;
    }
}