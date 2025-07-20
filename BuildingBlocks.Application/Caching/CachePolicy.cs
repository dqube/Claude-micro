namespace BuildingBlocks.Application.Caching;

public class CachePolicy
{
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public CachePriority Priority { get; set; } = CachePriority.Normal;
    public bool NeverExpires { get; set; } = false;

    public static CachePolicy Default => new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30)
    };

    public static CachePolicy WithAbsoluteExpiration(TimeSpan expiration) => new()
    {
        AbsoluteExpiration = expiration
    };

    public static CachePolicy WithSlidingExpiration(TimeSpan expiration) => new()
    {
        SlidingExpiration = expiration
    };

    public static CachePolicy NeverExpire => new()
    {
        NeverExpires = true
    };
}

public enum CachePriority
{
    Low,
    Normal,
    High,
    Critical
}