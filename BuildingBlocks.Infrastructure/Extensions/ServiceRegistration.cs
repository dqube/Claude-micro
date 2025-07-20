using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Repositories;
using BuildingBlocks.Infrastructure.Data.UnitOfWork;
using BuildingBlocks.Infrastructure.Data.Interceptors;
using BuildingBlocks.Infrastructure.Data.Migrations;
using BuildingBlocks.Infrastructure.Data.Seeding;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Application.Caching;
using BuildingBlocks.Infrastructure.Messaging.MessageBus;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Infrastructure.Authentication.JWT;
using BuildingBlocks.Infrastructure.Storage.Files;
using BuildingBlocks.Infrastructure.Serialization.Json;
using BuildingBlocks.Infrastructure.Configuration;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Application.Services;
using StackExchange.Redis;

namespace BuildingBlocks.Infrastructure.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataServices(configuration);
        services.AddCachingServices(configuration);
        services.AddMessagingServices();
        services.AddAuthenticationServices(configuration);
        services.AddStorageServices();
        services.AddSerializationServices();
        services.AddConfigurationServices();
        services.AddLoggingServices();

        return services;
    }

    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        // Register DbContext with interceptors
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
            
            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditInterceptor>(),
                serviceProvider.GetRequiredService<DomainEventInterceptor>(),
                serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

        // Register context abstraction
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Register repositories
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyRepository<,>));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register migration and seeding services
        services.AddScoped<IMigrationRunner, MigrationRunner>();
        services.AddScoped<IDataSeeder, DataSeederBase>();

        return services;
    }

    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register cache configuration
        var cacheConfig = new CacheConfiguration
        {
            KeyPrefix = configuration["Cache:KeyPrefix"] ?? "app:",
            DefaultExpiration = TimeSpan.FromMinutes(30)
        };
        services.AddSingleton(cacheConfig);

        // Register memory cache
        services.AddMemoryCache();

        // Register cache services (primary implementation)
        services.AddScoped<ICacheService, MemoryCacheService>();
        
        // Register additional cache implementations
        services.AddScoped<MemoryCacheService>();
        services.AddScoped<DistributedCacheService>();

        // Configure Redis if available
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddScoped<RedisCacheService>();
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        // Register cache utilities
        services.AddSingleton<CacheKeyGenerator>();

        return services;
    }

    public static IServiceCollection AddMessagingServices(this IServiceCollection services)
    {
        // Register message bus concrete implementation only
        services.AddSingleton<InMemoryMessageBus>();

        // Register domain event service
        services.AddScoped<IDomainEventService, DomainEventService>();

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register JWT configuration
        var jwtConfig = new JwtConfiguration
        {
            SecretKey = configuration["Jwt:SecretKey"] ?? "default-secret-key-change-in-production-min-32-chars",
            Issuer = configuration["Jwt:Issuer"] ?? "BuildingBlocks",
            Audience = configuration["Jwt:Audience"] ?? "BuildingBlocks-API",
            TokenLifetime = TimeSpan.FromMinutes(int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60"))
        };
        services.AddSingleton(jwtConfig);

        // Register JWT services
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }

    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        // Register file storage services
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }

    public static IServiceCollection AddSerializationServices(this IServiceCollection services)
    {
        // Register serialization services
        services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();

        return services;
    }

    public static IServiceCollection AddConfigurationServices(this IServiceCollection services)
    {
        // Register configuration services
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        return services;
    }

    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        // Register logging services
        services.AddScoped<ILoggerService, LoggerService>();

        return services;
    }
}