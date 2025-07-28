using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public class HttpHealthCheck : IHealthCheck
{
    private readonly HttpHealthCheckOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpHealthCheck> _logger;

    public HttpHealthCheck(
        HttpHealthCheckOptions options,
        HttpClient httpClient,
        ILogger<HttpHealthCheck> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

        // Add custom headers
        foreach (var header in _options.Headers)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            using var response = await _httpClient.GetAsync(_options.Url, cancellationToken);
            
            stopwatch.Stop();

            var statusCode = (int)response.StatusCode;
            var isExpectedStatusCode = _options.ExpectedStatusCodes.Contains(statusCode);

            var data = new Dictionary<string, object>
            {
                ["Url"] = _options.Url,
                ["StatusCode"] = statusCode,
                ["ResponseTimeMs"] = stopwatch.ElapsedMilliseconds,
                ["ExpectedStatusCodes"] = string.Join(", ", _options.ExpectedStatusCodes),
                ["IsExpectedStatusCode"] = isExpectedStatusCode
            };

            var status = isExpectedStatusCode ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy : Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
            var message = isExpectedStatusCode
                ? $"HTTP endpoint is healthy. Status: {statusCode}, Response time: {stopwatch.ElapsedMilliseconds}ms"
                : $"HTTP endpoint returned unexpected status code {statusCode}. Expected: {string.Join(", ", _options.ExpectedStatusCodes)}";

            _logger.LogDebug("HTTP health check completed for {Url}. Status: {Status}, StatusCode: {StatusCode}, ResponseTime: {ResponseTimeMs}ms",
                _options.Url, status, statusCode, stopwatch.ElapsedMilliseconds);

            return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(status, message, data: data);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning("HTTP health check timed out for {Url} after {TimeoutSeconds}s", _options.Url, _options.TimeoutSeconds);
            
            return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                $"HTTP endpoint timed out after {_options.TimeoutSeconds} seconds",
                ex,
                new Dictionary<string, object>
                {
                    ["Url"] = _options.Url,
                    ["TimeoutSeconds"] = _options.TimeoutSeconds
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP health check failed for {Url}", _options.Url);
            
            return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                $"HTTP endpoint check failed: {ex.Message}",
                ex,
                new Dictionary<string, object>
                {
                    ["Url"] = _options.Url,
                    ["ExceptionType"] = ex.GetType().Name
                });
        }
    }
}