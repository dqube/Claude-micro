using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Caching;
using AppICacheService = BuildingBlocks.Application.Caching.ICacheService;

namespace BuildingBlocks.Infrastructure.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<AppICacheService, BuildingBlocks.Infrastructure.Caching.MemoryCacheService>();
        return services;
    }

    public static IServiceCollection AddDistributedCache(this IServiceCollection services)
    {
        services.AddScoped<AppICacheService, BuildingBlocks.Infrastructure.Caching.DistributedCacheService>();
        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });
        services.AddScoped<AppICacheService, BuildingBlocks.Infrastructure.Caching.RedisCacheService>();
        return services;
    }
}