namespace BuildingBlocks.Infrastructure.External.HttpClients;

public class HttpClientConfiguration
{
    public string BaseAddress { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public RetryConfiguration Retry { get; set; } = new();
    public CircuitBreakerConfiguration CircuitBreaker { get; set; } = new();
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
}

public class RetryConfiguration
{
    public bool Enabled { get; set; } = true;
    public int MaxAttempts { get; set; } = 3;
    public int BaseDelayMilliseconds { get; set; } = 1000;
    public double BackoffMultiplier { get; set; } = 2.0;
    public int MaxDelayMilliseconds { get; set; } = 30000;
    public List<int> RetryStatusCodes { get; set; } = new() { 408, 429, 500, 502, 503, 504 };
}

public class CircuitBreakerConfiguration
{
    public bool Enabled { get; set; } = true;
    public int FailureThreshold { get; set; } = 5;
    public int SamplingDurationSeconds { get; set; } = 60;
    public int MinimumThroughput { get; set; } = 10;
    public int DurationOfBreakSeconds { get; set; } = 30;
}