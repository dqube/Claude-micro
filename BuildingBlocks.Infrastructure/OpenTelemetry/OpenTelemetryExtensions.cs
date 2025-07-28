using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Infrastructure.OpenTelemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var otelConfig = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConfiguration>() ?? new OpenTelemetryConfiguration();
        
        services.Configure<OpenTelemetryConfiguration>(configuration.GetSection("OpenTelemetry"));
        
        // Configure ActivitySource for custom activities
        services.AddSingleton(provider => new ActivitySource(otelConfig.ServiceName));

        return services;
    }
}