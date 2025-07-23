using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.Application.Security;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddApplicationLayer();
    }

    public static IServiceCollection AddSecurityContext(this IServiceCollection services)
    {
        services.AddScoped<SecurityContext>();
        services.AddScoped<UserContext>();
        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<CacheSettings>();
        return services;
    }

    public static IServiceCollection AddDistributedCaching(this IServiceCollection services, string connectionString)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });
        return services;
    }

    public static IHostBuilder UseApplication(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddApplicationLayer();
        });
    }

    public static IHost UseApplicationSecurity(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        using var scope = host.Services.CreateScope();
        var securityContext = scope.ServiceProvider.GetRequiredService<SecurityContext>();
        // Initialize security context if needed
        return host;
    }
}