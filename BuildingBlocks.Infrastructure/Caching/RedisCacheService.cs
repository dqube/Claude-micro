using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(ICacheKey key, CancellationToken cancellationToken = default) where T : class
    {
        return await GetAsync<T>(key.Key, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedValue = await _database.StringGetAsync(key);
            
            if (!cachedValue.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue!);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(ICacheKey key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        await SetAsync(key.Key, value, policy, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            TimeSpan? expiration = policy?.AbsoluteExpiration;
            await _database.StringSetAsync(key, serializedValue, expiration);
            
            _logger.LogDebug("Cache set for key: {Key}", key);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        await RemoveAsync(key.Key, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
            
            _logger.LogDebug("Cache removed by pattern: {Pattern}", pattern);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error removing cached values by pattern {Pattern}", pattern);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error removing cached values by pattern {Pattern}", pattern);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error removing cached values by pattern {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(key.Key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public Task RefreshAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        return RefreshAsync(key.Key, cancellationToken);
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyExpireAsync(key, TimeSpan.FromMinutes(30)); // Refresh with default expiry
            _logger.LogDebug("Cache refreshed for key: {Key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
            await server.FlushDatabaseAsync();
            
            _logger.LogDebug("Cache cleared");
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
    }
}