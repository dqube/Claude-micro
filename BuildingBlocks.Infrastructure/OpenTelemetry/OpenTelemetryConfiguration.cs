using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Infrastructure.OpenTelemetry;

public class OpenTelemetryConfiguration
{
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceVersion { get; set; } = "1.0.0";
    public bool UseConsoleExporter { get; set; } = true;
    public bool UseOtlpExporter { get; set; } = false;
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    
    public LoggingConfiguration Logging { get; set; } = new();
    public TracingConfiguration Tracing { get; set; } = new();
    public MetricsConfiguration Metrics { get; set; } = new();
}

public class LoggingConfiguration
{
    public bool IncludeFormattedMessage { get; set; } = true;
    public bool IncludeScopes { get; set; } = true;
}

public class TracingConfiguration
{
    public bool Enabled { get; set; } = true;
    public bool AspNetCore { get; set; } = true;
    public bool HttpClient { get; set; } = true;
    public bool EntityFrameworkCore { get; set; } = true;
}

public class MetricsConfiguration
{
    public bool Enabled { get; set; } = true;
    public bool AspNetCore { get; set; } = true;
    public bool HttpClient { get; set; } = true;
    public bool Runtime { get; set; } = true;
}