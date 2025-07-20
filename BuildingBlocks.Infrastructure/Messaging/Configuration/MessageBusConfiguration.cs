namespace BuildingBlocks.Infrastructure.Messaging.Configuration;

public class MessageBusConfiguration
{
    public string Provider { get; set; } = "InMemory";
    public string ConnectionString { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool EnableDeadLetterQueue { get; set; } = true;
    public int MaxConcurrentMessages { get; set; } = 10;
    public TimeSpan MessageTimeToLive { get; set; } = TimeSpan.FromHours(24);
}