using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.API.Configuration.Options;

public class RateLimitingOptions
{
    public const string ConfigurationSection = "RateLimiting";

    public bool Enabled { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int PermitLimit { get; set; } = 100;

    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    public int QueueLimit { get; set; } = 0;

    public RateLimitingPolicy Policy { get; set; } = RateLimitingPolicy.FixedWindow;

    public string PolicyName { get; set; } = "DefaultRateLimitPolicy";

    public Dictionary<string, EndpointRateLimitOptions> EndpointPolicies { get; set; } = new();
}

public class EndpointRateLimitOptions
{
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
    public RateLimitingPolicy Policy { get; set; }
}

public enum RateLimitingPolicy
{
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    Concurrency
}