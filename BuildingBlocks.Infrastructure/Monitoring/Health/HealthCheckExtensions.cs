using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Monitoring.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddComprehensiveHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthCheckConfig = configuration.GetSection("HealthChecks").Get<HealthCheckConfiguration>() 
            ?? new HealthCheckConfiguration();

        services.Configure<HealthCheckConfiguration>(configuration.GetSection("HealthChecks"));

        var healthChecksBuilder = services.AddHealthChecks();

        // Add Memory Health Check
        if (healthCheckConfig.Memory.Enabled)
        {
            services.Configure<MemoryHealthCheckOptions>(options =>
            {
                options.Enabled = healthCheckConfig.Memory.Enabled;
                options.Name = healthCheckConfig.Memory.Name;
                options.ThresholdBytes = healthCheckConfig.Memory.ThresholdBytes;
                options.Tags = healthCheckConfig.Memory.Tags;
            });

            healthChecksBuilder.AddCheck<MemoryHealthCheck>(
                healthCheckConfig.Memory.Name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                healthCheckConfig.Memory.Tags);
        }

        // Add Database Health Check - requires AspNetCore.HealthChecks.SqlServer package
        // For now, we'll skip this implementation until the package is confirmed available
        // if (healthCheckConfig.Database.Enabled)
        // {
        //     var connectionString = healthCheckConfig.Database.ConnectionString 
        //         ?? configuration.GetConnectionString("DefaultConnection");
        //     if (!string.IsNullOrEmpty(connectionString))
        //     {
        //         healthChecksBuilder.AddSqlServer(connectionString);
        //     }
        // }

        // Add HTTP Health Checks
        if (healthCheckConfig.Http.Enabled && !string.IsNullOrEmpty(healthCheckConfig.Http.Url))
        {
            healthChecksBuilder.AddTypeActivatedCheck<HttpHealthCheck>(
                healthCheckConfig.Http.Name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                healthCheckConfig.Http.Tags,
                healthCheckConfig.Http);
        }

        // Add Redis Health Check
        if (healthCheckConfig.Redis.Enabled && !string.IsNullOrEmpty(healthCheckConfig.Redis.ConnectionString))
        {
            // Redis health check requires AspNetCore.HealthChecks.Redis package
            // For now, we'll skip this implementation until the package is confirmed available
            // healthChecksBuilder.AddRedis(healthCheckConfig.Redis.ConnectionString);
        }

        // Add SMTP Health Check
        if (healthCheckConfig.Smtp.Enabled && !string.IsNullOrEmpty(healthCheckConfig.Smtp.Host))
        {
            healthChecksBuilder.AddTypeActivatedCheck<SmtpHealthCheck>(
                healthCheckConfig.Smtp.Name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                healthCheckConfig.Smtp.Tags,
                healthCheckConfig.Smtp);
        }

        return services;
    }

    public static IServiceCollection AddHttpHealthCheck(
        this IServiceCollection services,
        string name,
        string url,
        IEnumerable<string>? tags = null,
        int timeoutSeconds = 10,
        IEnumerable<int>? expectedStatusCodes = null)
    {
        var options = new HttpHealthCheckOptions
        {
            Name = name,
            Url = url,
            TimeoutSeconds = timeoutSeconds,
            ExpectedStatusCodes = expectedStatusCodes?.ToList() ?? new List<int> { 200 },
            Tags = tags?.ToList() ?? new List<string> { "http", "external" }
        };

        services.AddHttpClient<HttpHealthCheck>();
        
        return services.AddHealthChecks()
            .AddTypeActivatedCheck<HttpHealthCheck>(
                name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                options.Tags,
                options)
            .Services;
    }

    public static IServiceCollection AddSmtpHealthCheck(
        this IServiceCollection services,
        string name,
        string host,
        int port = 587,
        IEnumerable<string>? tags = null,
        int timeoutSeconds = 10)
    {
        var options = new SmtpHealthCheckOptions
        {
            Name = name,
            Host = host,
            Port = port,
            TimeoutSeconds = timeoutSeconds,
            Tags = tags?.ToList() ?? new List<string> { "smtp", "email" }
        };

        return services.AddHealthChecks()
            .AddTypeActivatedCheck<SmtpHealthCheck>(
                name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                options.Tags,
                options)
            .Services;
    }

    public static IServiceCollection AddMemoryHealthCheck(
        this IServiceCollection services,
        string name = "Memory",
        long thresholdBytes = 1024 * 1024 * 1024, // 1GB
        IEnumerable<string>? tags = null)
    {
        services.Configure<MemoryHealthCheckOptions>(options =>
        {
            options.Name = name;
            options.ThresholdBytes = thresholdBytes;
            options.Tags = tags?.ToList() ?? new List<string> { "memory", "system" };
        });

        return services.AddHealthChecks()
            .AddCheck<MemoryHealthCheck>(
                name,
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                tags?.ToArray() ?? Array.Empty<string>())
            .Services;
    }
}