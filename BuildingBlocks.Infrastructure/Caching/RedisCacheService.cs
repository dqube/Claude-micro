using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Infrastructure.Caching;

public partial class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    [LoggerMessage(LogLevel.Error, "Error getting cached value for key {Key}")]
    private static partial void LogGetError(ILogger logger, Exception exception, string key);

    [LoggerMessage(LogLevel.Debug, "Cache set for key: {Key}")]
    private static partial void LogCacheSet(ILogger logger, string key);

    [LoggerMessage(LogLevel.Error, "Error setting cache value for key: {Key}")]
    private static partial void LogSetError(ILogger logger, Exception exception, string key);

    [LoggerMessage(LogLevel.Debug, "Cache removed for key: {Key}")]
    private static partial void LogCacheRemoved(ILogger logger, string key);

    [LoggerMessage(LogLevel.Error, "Error removing cache value for key: {Key}")]
    private static partial void LogRemoveError(ILogger logger, Exception exception, string key);

    [LoggerMessage(LogLevel.Debug, "Cache removed by pattern: {Pattern}")]
    private static partial void LogCacheRemovedByPattern(ILogger logger, string pattern);

    [LoggerMessage(LogLevel.Error, "Error removing cached values by pattern {Pattern}")]
    private static partial void LogRemoveByPatternError(ILogger logger, Exception exception, string pattern);

    [LoggerMessage(LogLevel.Error, "Error checking cache existence for key: {Key}")]
    private static partial void LogExistsError(ILogger logger, Exception exception, string key);

    [LoggerMessage(LogLevel.Debug, "Cache refreshed for key: {Key}")]
    private static partial void LogCacheRefreshed(ILogger logger, string key);

    [LoggerMessage(LogLevel.Error, "Error refreshing cache for key: {Key}")]
    private static partial void LogRefreshError(ILogger logger, Exception exception, string key);

    [LoggerMessage(LogLevel.Debug, "Cache cleared")]
    private static partial void LogCacheCleared(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Error clearing cache")]
    private static partial void LogClearError(ILogger logger, Exception exception);

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        ArgumentNullException.ThrowIfNull(connectionMultiplexer);
        ArgumentNullException.ThrowIfNull(logger);
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(ICacheKey key, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
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
            LogGetError(_logger, ex, key);
            return default;
        }
        catch (RedisException ex)
        {
            LogGetError(_logger, ex, key);
            return default;
        }
        catch (InvalidOperationException ex)
        {
            LogGetError(_logger, ex, key);
            return default;
        }
    }

    public async Task SetAsync<T>(ICacheKey key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(key);
        await SetAsync(key.Key, value, policy, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            TimeSpan? expiration = policy?.AbsoluteExpiration;
            await _database.StringSetAsync(key, serializedValue, expiration);
            
            LogCacheSet(_logger, key);
        }
        catch (JsonException ex)
        {
            LogSetError(_logger, ex, key);
        }
        catch (RedisException ex)
        {
            LogSetError(_logger, ex, key);
        }
        catch (InvalidOperationException ex)
        {
            LogSetError(_logger, ex, key);
        }
    }

    public async Task RemoveAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        await RemoveAsync(key.Key, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            
            LogCacheRemoved(_logger, key);
        }
        catch (RedisException ex)
        {
            LogRemoveError(_logger, ex, key);
        }
        catch (InvalidOperationException ex)
        {
            LogRemoveError(_logger, ex, key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
            var keys = server.KeysAsync(pattern: pattern);
            
            await foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
            
            LogCacheRemovedByPattern(_logger, pattern);
        }
        catch (RedisException ex)
        {
            LogRemoveByPatternError(_logger, ex, pattern);
        }
        catch (InvalidOperationException ex)
        {
            LogRemoveByPatternError(_logger, ex, pattern);
        }
        catch (ArgumentException ex)
        {
            LogRemoveByPatternError(_logger, ex, pattern);
        }
    }

    public async Task<bool> ExistsAsync(ICacheKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
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
            LogExistsError(_logger, ex, key);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            LogExistsError(_logger, ex, key);
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
        try
        {
            await _database.KeyExpireAsync(key, TimeSpan.FromMinutes(30)); // Refresh with default expiry
            LogCacheRefreshed(_logger, key);
        }
        catch (RedisException ex)
        {
            LogRefreshError(_logger, ex, key);
        }
        catch (InvalidOperationException ex)
        {
            LogRefreshError(_logger, ex, key);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
            await server.FlushDatabaseAsync();
            
            LogCacheCleared(_logger);
        }
        catch (RedisException ex)
        {
            LogClearError(_logger, ex);
        }
        catch (InvalidOperationException ex)
        {
            LogClearError(_logger, ex);
        }
    }
}