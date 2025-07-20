namespace BuildingBlocks.Application.Caching;

public class CacheKey : ICacheKey
{
    public CacheKey(string key, TimeSpan? expiration = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Expiration = expiration;
    }

    public string Key { get; }
    public TimeSpan? Expiration { get; }

    public static CacheKey Create(string key) => new(key);
    public static CacheKey Create(string key, TimeSpan expiration) => new(key, expiration);
    
    public override string ToString() => Key;
    
    public static implicit operator string(CacheKey cacheKey) => cacheKey.Key;
}