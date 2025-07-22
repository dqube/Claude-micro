using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using BuildingBlocks.API.Health.Reporters;

namespace BuildingBlocks.API.Health.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy("API is running"))
            .AddCheck("memory", () =>
            {
                var allocatedBytes = GC.GetTotalMemory(false);
                var data = new Dictionary<string, object>
                {
                    ["allocated"] = allocatedBytes,
                    ["gen0"] = GC.CollectionCount(0),
                    ["gen1"] = GC.CollectionCount(1),
                    ["gen2"] = GC.CollectionCount(2)
                };
                
                // Consider unhealthy if more than 1GB allocated
                if (allocatedBytes > 1024 * 1024 * 1024)
                {
                    return HealthCheckResult.Unhealthy("High memory usage detected", data: data);
                }
                
                return HealthCheckResult.Healthy("Memory usage is normal", data);
            });

        services.AddSingleton<IHealthCheckPublisher, JsonHealthReporter>();
        
        return services;
    }

    public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services, string connectionString, string name = "database")
    {
        services.AddHealthChecks()
            .AddSqlServer(connectionString, name: name, tags: new[] { "database" });
        
        return services;
    }

    public static IServiceCollection AddRedisHealthCheck(this IServiceCollection services, string connectionString, string name = "redis")
    {
        services.AddHealthChecks()
            .AddRedis(connectionString, name: name, tags: new[] { "cache" });
        
        return services;
    }

    public static IServiceCollection AddHttpHealthCheck(this IServiceCollection services, string uri, string name)
    {
        services.AddHealthChecks()
            .AddUrlGroup(new Uri(uri), name, tags: new[] { "external" });
        
        return services;
    }

    public static IServiceCollection AddCustomHealthCheck<T>(this IServiceCollection services, string name, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null)
        where T : class, IHealthCheck
    {
        services.AddSingleton<T>();
        services.AddHealthChecks()
            .AddCheck<T>(name, failureStatus, tags);
        
        return services;
    }
}