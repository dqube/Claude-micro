using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Sockets;

namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly SmtpHealthCheckOptions _options;
    private readonly ILogger<SmtpHealthCheck> _logger;

    public SmtpHealthCheck(
        SmtpHealthCheckOptions options,
        ILogger<SmtpHealthCheck> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            using var tcpClient = new TcpClient();
            var connectTask = tcpClient.ConnectAsync(_options.Host, _options.Port);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(_options.TimeoutSeconds), cancellationToken);

            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                stopwatch.Stop();
                
                _logger.LogWarning("SMTP health check timed out for {Host}:{Port} after {TimeoutSeconds}s", 
                    _options.Host, _options.Port, _options.TimeoutSeconds);
                
                return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                    Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    $"SMTP server connection timed out after {_options.TimeoutSeconds} seconds",
                    data: new Dictionary<string, object>
                    {
                        ["Host"] = _options.Host,
                        ["Port"] = _options.Port,
                        ["TimeoutSeconds"] = _options.TimeoutSeconds,
                        ["ResponseTimeMs"] = stopwatch.ElapsedMilliseconds
                    });
            }

            await connectTask; // This will throw if the connection failed
            stopwatch.Stop();

            var data = new Dictionary<string, object>
            {
                ["Host"] = _options.Host,
                ["Port"] = _options.Port,
                ["ResponseTimeMs"] = stopwatch.ElapsedMilliseconds,
                ["Connected"] = tcpClient.Connected
            };

            var message = $"SMTP server is reachable. Host: {_options.Host}:{_options.Port}, Response time: {stopwatch.ElapsedMilliseconds}ms";

            _logger.LogDebug("SMTP health check completed for {Host}:{Port}. Status: Healthy, ResponseTime: {ResponseTimeMs}ms",
                _options.Host, _options.Port, stopwatch.ElapsedMilliseconds);

            return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy, message, data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP health check failed for {Host}:{Port}", _options.Host, _options.Port);
            
            return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                $"SMTP server connection failed: {ex.Message}",
                ex,
                new Dictionary<string, object>
                {
                    ["Host"] = _options.Host,
                    ["Port"] = _options.Port,
                    ["ExceptionType"] = ex.GetType().Name
                });
        }
    }
}