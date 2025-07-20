namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public interface IHealthCheckService
{
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    Task<HealthCheckResult> CheckHealthAsync(string name, CancellationToken cancellationToken = default);
    Task<Dictionary<string, HealthCheckResult>> CheckAllHealthAsync(CancellationToken cancellationToken = default);
}

public class HealthCheckResult
{
    public HealthStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public Exception? Exception { get; set; }
}

public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}