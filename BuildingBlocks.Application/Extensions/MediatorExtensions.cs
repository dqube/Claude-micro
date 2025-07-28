using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Dispatchers;
using BuildingBlocks.Application.CQRS.Mediator;

namespace BuildingBlocks.Application.Extensions
{
    // Extension methods for registering mediator, handlers, and behaviors
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            // Register dispatchers
            services.AddDispatchers();

            // Register mediator
            services.AddMediatorOnly();

            // Register handlers from BuildingBlocks assembly only
            services.AddCommandHandlers();
            services.AddQueryHandlers();
            services.AddEventHandlers();
            services.AddMessageHandlers();

            return services;
        }

        public static IServiceCollection AddMediatorBehaviors(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
            return services;
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<ICommand>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<ICommand>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<IQuery<object>>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<IEvent>()
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }

        public static IServiceCollection AddDispatchers(this IServiceCollection services)
        {
            // Register dispatchers
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IMessageDispatcher, MessageDispatcher>();
            return services;
        }

        public static IServiceCollection AddMediatorOnly(this IServiceCollection services)
        {
            // Register mediator (assumes dispatchers are already registered)
            services.AddScoped<IMediator, BuildingBlocks.Application.CQRS.Mediator.Mediator>();
            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<IEvent>()
                .AddClasses(classes => classes.AssignableTo<BuildingBlocks.Application.Messaging.IMessageHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<IEvent>()
                .AddClasses(classes => classes.AssignableTo<BuildingBlocks.Application.Messaging.IMessageHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddMessageHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<BuildingBlocks.Application.Messaging.IMessageHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<BuildingBlocks.Application.Messaging.IMessageHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        /// <summary>
        /// Adds mediator with dispatchers and registers handlers from specified assemblies
        /// </summary>
        public static IServiceCollection AddMediatorWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Register dispatchers and mediator
            services.AddDispatchers();
            services.AddMediatorOnly();

            // Register handlers from specified assemblies
            services.AddCommandHandlers(assemblies);
            services.AddQueryHandlers(assemblies);
            services.AddEventHandlers(assemblies);
            services.AddMessageHandlers(assemblies);

            return services;
        }

        /// <summary>
        /// Adds all handlers from specified assemblies (without mediator/dispatchers)
        /// </summary>
        public static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddCommandHandlers(assemblies);
            services.AddQueryHandlers(assemblies);
            services.AddEventHandlers(assemblies);
            services.AddMessageHandlers(assemblies);

            return services;
        }
    }
}