using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly MemoryHealthCheckOptions _options;
    private readonly ILogger<MemoryHealthCheck> _logger;

    public MemoryHealthCheck(
        IOptions<MemoryHealthCheckOptions> options,
        ILogger<MemoryHealthCheck> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var workingSet = process.WorkingSet64;
            var privateMemory = process.PrivateMemorySize64;

            var data = new Dictionary<string, object>
            {
                ["WorkingSetBytes"] = workingSet,
                ["PrivateMemoryBytes"] = privateMemory,
                ["WorkingSetMB"] = Math.Round(workingSet / 1024.0 / 1024.0, 2),
                ["PrivateMemoryMB"] = Math.Round(privateMemory / 1024.0 / 1024.0, 2),
                ["ThresholdBytes"] = _options.ThresholdBytes,
                ["ThresholdMB"] = Math.Round(_options.ThresholdBytes / 1024.0 / 1024.0, 2)
            };

            var status = workingSet > _options.ThresholdBytes 
                ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded 
                : Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;

            var message = status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy
                ? $"Memory usage is within acceptable limits ({Math.Round(workingSet / 1024.0 / 1024.0, 2)} MB)"
                : $"Memory usage is high ({Math.Round(workingSet / 1024.0 / 1024.0, 2)} MB exceeds threshold of {Math.Round(_options.ThresholdBytes / 1024.0 / 1024.0, 2)} MB)";

            _logger.LogDebug("Memory health check completed. Status: {Status}, WorkingSet: {WorkingSetMB} MB", 
                status, Math.Round(workingSet / 1024.0 / 1024.0, 2));

            return Task.FromResult(new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(status, message, data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Memory health check failed");
            return Task.FromResult(new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, 
                "Failed to check memory usage", 
                ex));
        }
    }
}