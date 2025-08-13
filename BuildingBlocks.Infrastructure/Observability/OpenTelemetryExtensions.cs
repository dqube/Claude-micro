using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;
using BuildingBlocks.Infrastructure.Logging;

namespace BuildingBlocks.Infrastructure.Observability;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryObservability(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var openTelemetryOptions = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>() ?? new OpenTelemetryOptions();
        
        // Configure redaction from multiple sources
        var redactionOptions = new RedactionOptions();
        
        // Check if redaction is explicitly disabled in either section
        var otelRedactionEnabled = configuration.GetValue<bool?>("OpenTelemetry:Redaction:Enabled");
        var redactionSettingsEnabled = configuration.GetValue<bool?>("RedactionSettings:Enabled");
        
        // If either section explicitly disables redaction, disable it entirely
        if (otelRedactionEnabled == false || redactionSettingsEnabled == false)
        {
            redactionOptions.Enabled = false;
        }
        else
        {
            // Start with a fresh set of sensitive fields to avoid conflicts with defaults
            var configuredSensitiveFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // First, try OpenTelemetry:Redaction section
            var otelRedactionSection = configuration.GetSection("OpenTelemetry:Redaction");
            if (otelRedactionSection.Exists())
            {
                otelRedactionSection.Bind(redactionOptions);
                
                // Get sensitive fields from OpenTelemetry section
                var otelSensitiveFields = otelRedactionSection.GetSection("SensitiveFields").Get<string[]>();
                if (otelSensitiveFields?.Length > 0)
                {
                    foreach (var field in otelSensitiveFields)
                    {
                        configuredSensitiveFields.Add(field);
                    }
                }
            }
            
            // Then, merge settings from RedactionSettings section
            var redactionSettingsSection = configuration.GetSection("RedactionSettings");
            if (redactionSettingsSection.Exists())
            {
                // Only override enabled if not already explicitly set
                if (redactionSettingsEnabled.HasValue && otelRedactionEnabled != true)
                {
                    redactionOptions.Enabled = redactionSettingsEnabled.Value;
                }
                
                var redactionText = redactionSettingsSection.GetValue<string>("RedactionText");
                if (!string.IsNullOrEmpty(redactionText))
                {
                    redactionOptions.RedactionText = redactionText;
                }
                
                // Add sensitive fields from RedactionSettings
                var sensitiveFields = redactionSettingsSection.GetSection("SensitiveFields").Get<string[]>();
                if (sensitiveFields?.Length > 0)
                {
                    foreach (var field in sensitiveFields)
                    {
                        configuredSensitiveFields.Add(field);
                    }
                }
                
                // Handle redaction character
                var redactionChar = redactionSettingsSection.GetValue<string>("RedactionCharacter");
                if (!string.IsNullOrEmpty(redactionChar))
                {
                    redactionOptions.RedactionText = new string(redactionChar[0], 8);
                }
            }
            
            // If we have configured sensitive fields, use them instead of defaults
            if (configuredSensitiveFields.Count > 0)
            {
                // Clear defaults and use only configured fields
                redactionOptions.SensitiveFields.Clear();
                foreach (var field in configuredSensitiveFields)
                {
                    redactionOptions.SensitiveFields.Add(field);
                }
            }
            // If no configured fields, keep the defaults from RedactionOptions constructor
        }
        
        services.AddSingleton(redactionOptions);
        services.AddSingleton<IDataRedactionService, DataRedactionService>();
        
        // Use configured service name or fallback to assembly name
        var serviceName = !string.IsNullOrEmpty(openTelemetryOptions.ServiceName) 
            ? openTelemetryOptions.ServiceName 
            : Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
            
        var serviceVersion = !string.IsNullOrEmpty(openTelemetryOptions.ServiceVersion)
            ? openTelemetryOptions.ServiceVersion
            : Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";

        // Configure Resource
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                ["service.environment"] = environment.EnvironmentName,
                ["service.instance.id"] = Environment.MachineName
            });

        // Add OpenTelemetry
        var otelBuilder = services.AddOpenTelemetry();
        
        // Configure Tracing
        if (openTelemetryOptions.Tracing.Enabled)
        {
            otelBuilder.WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)
                    .AddSource(serviceName)
                    .AddSource("BuildingBlocks.*");
                
                // Add additional sources from configuration
                foreach (var source in openTelemetryOptions.Tracing.AdditionalSources)
                {
                    builder.AddSource(source);
                }
                
                // Add ASP.NET Core instrumentation if enabled
                if (openTelemetryOptions.Tracing.AspNetCore)
                {
                    builder.AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = httpContext =>
                        {
                            // Skip health check and metrics endpoints
                            var path = httpContext.Request.Path.Value;
                            return !path?.Contains("/health", StringComparison.OrdinalIgnoreCase) == true &&
                                   !path?.Contains("/metrics", StringComparison.OrdinalIgnoreCase) == true;
                        };
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request.method", request.Method);
                            activity.SetTag("http.request.scheme", request.Scheme);
                            activity.SetTag("http.request.host", request.Host.Value);
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            activity.SetTag("http.response.status_code", response.StatusCode);
                        };
                    });
                }
                
                // Add HTTP client instrumentation if enabled
                if (openTelemetryOptions.Tracing.HttpClient)
                {
                    builder.AddHttpClientInstrumentation();
                }
                
                // Add Entity Framework Core instrumentation if enabled
                if (openTelemetryOptions.Tracing.EntityFrameworkCore)
                {
                    builder.AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.operation", command.CommandType.ToString());
                        };
                    });
                }
                
                builder.SetSampler(new AlwaysOnSampler());

                // Add redaction processor if enabled
                if (redactionOptions.Enabled)
                {
                    builder.AddProcessor(serviceProvider => new RedactionActivityProcessor(
                        serviceProvider.GetRequiredService<IDataRedactionService>()));
                }

                // Add exporters based on configuration
                if (openTelemetryOptions.Exporters.Console.Enabled)
                {
                    builder.AddConsoleExporter();
                }

                if (openTelemetryOptions.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Endpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(openTelemetryOptions.Exporters.Otlp.Endpoint);
                        if (!string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Headers))
                        {
                            options.Headers = openTelemetryOptions.Exporters.Otlp.Headers;
                        }
                    });
                }
            });
        }
        
        // Configure Metrics
        if (openTelemetryOptions.Metrics.Enabled)
        {
            otelBuilder.WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)
                    .AddMeter(serviceName)
                    .AddMeter("BuildingBlocks.*");
                
                // Add additional meters from configuration
                foreach (var meter in openTelemetryOptions.Metrics.AdditionalMeters)
                {
                    builder.AddMeter(meter);
                }
                
                // Add ASP.NET Core instrumentation if enabled
                if (openTelemetryOptions.Metrics.AspNetCore)
                {
                    builder.AddAspNetCoreInstrumentation();
                }
                
                // Add HTTP client instrumentation if enabled
                if (openTelemetryOptions.Metrics.HttpClient)
                {
                    builder.AddHttpClientInstrumentation();
                }

                // Add exporters based on configuration
                if (openTelemetryOptions.Exporters.Console.Enabled)
                {
                    builder.AddConsoleExporter();
                }

                if (openTelemetryOptions.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Endpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(openTelemetryOptions.Exporters.Otlp.Endpoint);
                        if (!string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Headers))
                        {
                            options.Headers = openTelemetryOptions.Exporters.Otlp.Headers;
                        }
                    });
                }
            });
        }

        // Configure Logging with OpenTelemetry
        if (openTelemetryOptions.Logging.Enabled)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                
                if (environment.IsDevelopment())
                {
                    builder.AddConsole();
                    builder.AddDebug();
                }

                builder.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resourceBuilder);
                    options.IncludeFormattedMessage = openTelemetryOptions.Logging.IncludeFormattedMessage;
                    options.IncludeScopes = openTelemetryOptions.Logging.IncludeScopes;
                    options.ParseStateValues = openTelemetryOptions.Logging.ParseStateValues;

                    // Add redaction processor if enabled
                    if (redactionOptions.Enabled)
                    {
                        options.AddProcessor(serviceProvider => new RedactionLogProcessor(
                            serviceProvider.GetRequiredService<IDataRedactionService>()));
                    }

                    // Add exporters based on configuration
                    if (openTelemetryOptions.Exporters.Console.Enabled)
                    {
                        options.AddConsoleExporter();
                    }

                    if (openTelemetryOptions.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Endpoint))
                    {
                        options.AddOtlpExporter(exporterOptions =>
                        {
                            exporterOptions.Endpoint = new Uri(openTelemetryOptions.Exporters.Otlp.Endpoint);
                            if (!string.IsNullOrEmpty(openTelemetryOptions.Exporters.Otlp.Headers))
                            {
                                exporterOptions.Headers = openTelemetryOptions.Exporters.Otlp.Headers;
                            }
                        });
                    }
                });
            });
        }
        
        // Register ActivitySource for custom activities
        services.AddSingleton(provider => new ActivitySource(serviceName));
        
        // Register configuration for other services
        services.Configure<OpenTelemetryOptions>(configuration.GetSection("OpenTelemetry"));

        return services;
    }

    public static IServiceCollection AddCustomMetrics(this IServiceCollection services)
    {
        services.AddSingleton<IMetricsService, MetricsService>();
        return services;
    }
}

public class OpenTelemetryOptions
{
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceVersion { get; set; } = "1.0.0";
    
    public ExportersOptions Exporters { get; set; } = new();
    public LoggingOptions Logging { get; set; } = new();
    public TracingOptions Tracing { get; set; } = new();
    public MetricsOptions Metrics { get; set; } = new();
    public RedactionOptions Redaction { get; set; } = new();
}

public class ExportersOptions
{
    public ConsoleExporterOptions Console { get; set; } = new();
    public OtlpExporterOptions Otlp { get; set; } = new();
}

public class ConsoleExporterOptions
{
    public bool Enabled { get; set; } = true;
}

public class OtlpExporterOptions
{
    public bool Enabled { get; set; } = false;
    public string Endpoint { get; set; } = string.Empty;
    public string Headers { get; set; } = string.Empty;
}

public class LoggingOptions
{
    public bool Enabled { get; set; } = true;
    public bool IncludeFormattedMessage { get; set; } = true;
    public bool IncludeScopes { get; set; } = true;
    public bool ParseStateValues { get; set; } = true;
}

public class TracingOptions
{
    public bool Enabled { get; set; } = true;
    public bool AspNetCore { get; set; } = true;
    public bool HttpClient { get; set; } = true;
    public bool EntityFrameworkCore { get; set; } = true;
    public string[] AdditionalSources { get; set; } = [];
}

public class MetricsOptions
{
    public bool Enabled { get; set; } = true;
    public bool AspNetCore { get; set; } = true;
    public bool HttpClient { get; set; } = true;
    public bool Runtime { get; set; } = true;
    public string[] AdditionalMeters { get; set; } = [];
}

public interface IMetricsService
{
    void IncrementCounter(string name, int value = 1, params KeyValuePair<string, object?>[] tags);
    void RecordValue(string name, double value, params KeyValuePair<string, object?>[] tags);
    void RecordHistogram(string name, double value, params KeyValuePair<string, object?>[] tags);
}

public class MetricsService : IMetricsService, IDisposable
{
    private readonly System.Diagnostics.Metrics.Meter _meter;
    private bool _disposed;

    public MetricsService()
    {
        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
        _meter = new System.Diagnostics.Metrics.Meter(serviceName);
    }

    public void IncrementCounter(string name, int value = 1, params KeyValuePair<string, object?>[] tags)
    {
        var counter = _meter.CreateCounter<int>(name);
        counter.Add(value, tags);
    }

    public void RecordValue(string name, double value, params KeyValuePair<string, object?>[] tags)
    {
        var gauge = _meter.CreateObservableGauge(name, () => value);
    }

    public void RecordHistogram(string name, double value, params KeyValuePair<string, object?>[] tags)
    {
        var histogram = _meter.CreateHistogram<double>(name);
        histogram.Record(value, tags);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _meter?.Dispose();
            _disposed = true;
        }
    }
}