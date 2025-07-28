using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Services;
using BuildingBlocks.Application.Validation;
using BuildingBlocks.Application.Sagas;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Domain.DomainEvents;

namespace BuildingBlocks.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Register application services
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register saga support
        services.AddSagas();

        // Register inbox and outbox support
        services.AddInboxOutboxSupport();

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }

    public static IServiceCollection AddPerformanceMonitoring(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        return services;
    }

    public static IServiceCollection AddTransactionSupport(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        return services;
    }

    public static IServiceCollection AddInboxOutboxSupport(this IServiceCollection services)
    {
        // Register inbox services
        services.AddScoped<IInboxProcessor, InboxProcessor>();
        services.AddSingleton<InboxOptions>();
        services.AddHostedService<InboxBackgroundService>();

        // Register outbox services
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddSingleton<OutboxOptions>();
        services.AddHostedService<OutboxBackgroundService>();

        return services;
    }

    public static IServiceCollection AddInboxSupport(this IServiceCollection services)
    {
        services.AddScoped<IInboxProcessor, InboxProcessor>();
        services.AddSingleton<InboxOptions>();
        services.AddHostedService<InboxBackgroundService>();
        return services;
    }

    public static IServiceCollection AddOutboxSupport(this IServiceCollection services)
    {
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddSingleton<OutboxOptions>();
        services.AddHostedService<OutboxBackgroundService>();
        return services;
    }
}