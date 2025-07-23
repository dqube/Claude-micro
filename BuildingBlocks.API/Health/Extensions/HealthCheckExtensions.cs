using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Health.Reporters;

namespace BuildingBlocks.API.Health.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
            .AddCheck("memory", () =>
            {
                var allocated = GC.GetTotalMemory(forceFullCollection: false);
                var threshold = 100 * 1024 * 1024; // 100 MB threshold
                
                return allocated < threshold 
                    ? HealthCheckResult.Healthy($"Memory usage: {allocated / 1024 / 1024} MB")
                    : HealthCheckResult.Degraded($"High memory usage: {allocated / 1024 / 1024} MB");
            });

        return services;
    }

    public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services, string connectionString, string name = "database")
    {
        //services.AddHealthChecks()
        //    .AddSqlServer(connectionString, name: name);

        return services;
    }

    public static IServiceCollection AddRedisHealthCheck(this IServiceCollection services, string connectionString, string name = "redis")
    {
        //services.AddHealthChecks()
        //    .AddRedis(connectionString, name: name);

        return services;
    }

    public static IServiceCollection AddUrlHealthCheck(this IServiceCollection services, string url, string name = "external_service")
    {
        //services.AddHealthChecks()
        //    .AddUrlGroup(new Uri(url), name: name);

        return services;
    }
}

public static class HealthEndpointExtensions
{
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = SimpleHealthReporter.WriteResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = JsonHealthReporter.WriteResponse
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false, // Only checks if the app is running
            ResponseWriter = SimpleHealthReporter.WriteResponse
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks("/health/details", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = JsonHealthReporter.WriteResponse
            });
        }

        return app;
    }
}