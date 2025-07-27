using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Repositories;
using BuildingBlocks.Infrastructure.Data.UnitOfWork;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Application.Caching;
using BuildingBlocks.Infrastructure.Messaging.MessageBus;
using BuildingBlocks.Infrastructure.Authentication.JWT;
using BuildingBlocks.Infrastructure.Storage.Files;
using BuildingBlocks.Infrastructure.Serialization.Json;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Infrastructure.Services;

namespace BuildingBlocks.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDataLayer();
        services.AddCaching();
        services.AddMessaging();
        services.AddAuthentication();
        services.AddSerialization();
        
        return services;
    }

    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {
        services.AddScoped<IDbContext, ApplicationDbContext>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register inbox and outbox services
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<IOutboxService, OutboxService>();
        
        return services;
    }

    public static IServiceCollection AddDatabase<TContext>(
        this IServiceCollection services,
        string connectionString)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseSqlServer(connectionString));
        
        return services;
    }

    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<CacheConfiguration>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        
        return services;
    }

    public static IServiceCollection AddDistributedCaching(
        this IServiceCollection services,
        string? redisConnectionString = null)
    {
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
        
        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, InMemoryMessageBus>();
        
        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<JwtConfiguration>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        
        return services;
    }

    public static IServiceCollection AddFileStorage(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        
        return services;
    }

    public static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        
        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        // Background services would be added here
        return services;
    }

    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        // Monitoring services would be added here
        return services;
    }
}