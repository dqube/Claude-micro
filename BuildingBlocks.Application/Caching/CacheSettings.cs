namespace BuildingBlocks.Application.Caching;

public class CacheSettings
{
    public bool EnableCaching { get; set; } = true;
    public string DefaultConnectionString { get; set; } = string.Empty;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public int DefaultSlidingExpirationMinutes { get; set; } = 15;
    public bool EnableDistributedCaching { get; set; } = false;
    public string? RedisConnectionString { get; set; }
    public string CacheKeyPrefix { get; set; } = "BuildingBlocks:";
    public bool CompressValues { get; set; } = true;
    public int MaxCacheSize { get; set; } = 1000;
}