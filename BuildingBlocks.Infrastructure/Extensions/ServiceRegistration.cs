using System.Globalization;
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
using BuildingBlocks.Infrastructure.Messaging.Kafka;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using BuildingBlocks.Infrastructure.Authentication.JWT;
using BuildingBlocks.Infrastructure.Storage.Files;
using BuildingBlocks.Infrastructure.Serialization.Json;
using BuildingBlocks.Infrastructure.Configuration;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Infrastructure.Observability;
using BuildingBlocks.Infrastructure.Communication.Email;
using BuildingBlocks.Infrastructure.External.HttpClients;
using BuildingBlocks.Infrastructure.Monitoring.Health;
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
        services.AddMessagingServices(configuration);
        services.AddAuthenticationServices(configuration);
        services.AddStorageServices();
        services.AddSerializationServices();
        services.AddConfigurationServices();
        services.AddLoggingServices();
        services.AddOpenTelemetryObservability(configuration, new DevelopmentHostEnvironment());
        services.AddEmailServices(configuration);
        services.AddHttpClientServices(configuration);
        services.AddComprehensiveHealthChecks(configuration);

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

        return services;
    }

    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
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

        return services;
    }

    /// <summary>
    /// Adds messaging services based on configuration provider (InMemory or Kafka)
    /// </summary>
    public static IServiceCollection AddMessagingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Read the messaging provider from configuration
        var messagingSection = configuration.GetSection("Messaging");
        var provider = messagingSection["Provider"] ?? "InMemory";

        if (provider.Equals("Kafka", StringComparison.OrdinalIgnoreCase))
        {
            // Register Kafka configuration
            services.Configure<KafkaConfiguration>(configuration.GetSection(KafkaConfiguration.SectionName));
            
            // Register Kafka message bus for Application.Messaging.IMessageBus interface
            services.AddSingleton<BuildingBlocks.Application.Messaging.IMessageBus, KafkaMessageBus>();
            
            // Register Kafka publisher and subscriber
            services.AddSingleton<KafkaMessagePublisher>();
            services.AddSingleton<KafkaMessageSubscriber>();
            
            // Register concrete implementation
            services.AddSingleton<KafkaMessageBus>();
            
            // Register subscription builder
            services.AddSingleton<KafkaMessageSubscriptionBuilder>();
        }
        else
        {
            // Register InMemory message bus (default)
            services.AddSingleton<InMemoryMessageBus>();
        }

        // Register domain event service
        services.AddScoped<IDomainEventService, DomainEventService>();

        return services;
    }

    /// <summary>
    /// Legacy method for backward compatibility - defaults to InMemory
    /// </summary>
    public static IServiceCollection AddMessagingServices(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryMessageBus>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        return services;
    }

    /// <summary>
    /// Adds Kafka message bus with fluent configuration and subscription builder
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Action to configure Kafka options</param>
    /// <param name="configureSubscriptions">Action to configure message subscriptions</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        Action<KafkaConfiguration> configureOptions,
        Action<KafkaMessageSubscriptionBuilder>? configureSubscriptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        // Configure Kafka
        services.Configure(configureOptions);

        // Register Kafka message bus
        services.AddSingleton<BuildingBlocks.Application.Messaging.IMessageBus, KafkaMessageBus>();
        services.AddSingleton<KafkaMessagePublisher>();
        services.AddSingleton<KafkaMessageSubscriber>();
        services.AddSingleton<KafkaMessageBus>();
        services.AddSingleton<KafkaMessageSubscriptionBuilder>();

        // If subscriptions are configured, store them for later activation
        if (configureSubscriptions != null)
        {
            services.AddSingleton<Action<KafkaMessageSubscriptionBuilder>>(configureSubscriptions);
        }

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        // Register JWT configuration
        var jwtConfig = new JwtConfiguration
        {
            SecretKey = configuration["Jwt:SecretKey"] ?? "default-secret-key-change-in-production-min-32-chars",
            Issuer = configuration["Jwt:Issuer"] ?? "BuildingBlocks",
            Audience = configuration["Jwt:Audience"] ?? "BuildingBlocks-API",
            TokenLifetime = TimeSpan.FromMinutes(int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60", CultureInfo.InvariantCulture))
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

    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        // Register email configuration
        services.Configure<EmailConfiguration>(configuration.GetSection("Email"));
        
        // Register email services
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }

    public static IServiceCollection AddHttpClientServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        // Register HTTP client configuration
        services.Configure<HttpClientConfiguration>(configuration.GetSection("HttpClient"));
        
        // Register HTTP client with retry policies
        services.AddHttpClient<HttpClientService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                UseCookies = false
            });
        
        // Register the service interface
        services.AddScoped<IHttpClientService, HttpClientService>();

        return services;
    }
}