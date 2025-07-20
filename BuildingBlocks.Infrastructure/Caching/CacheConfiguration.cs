namespace BuildingBlocks.Infrastructure.Caching;

public class CacheConfiguration
{
    public string KeyPrefix { get; set; } = "BuildingBlocks:";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableDistributedCaching { get; set; } = false;
    public string? RedisConnectionString { get; set; }
    public bool CompressLargeValues { get; set; } = true;
    public int CompressionThreshold { get; set; } = 1024; // bytes
    public bool EnableLogging { get; set; } = true;
    public int MaxKeyLength { get; set; } = 250;
    public bool UseAbsoluteExpiration { get; set; } = true;
    public bool UseSlidingExpiration { get; set; } = false;
    public TimeSpan SlidingExpirationDuration { get; set; } = TimeSpan.FromMinutes(20);
}