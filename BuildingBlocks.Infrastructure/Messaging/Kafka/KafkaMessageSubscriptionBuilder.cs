using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Messaging.Kafka;

/// <summary>
/// Fluent builder for configuring Kafka message subscriptions
/// </summary>
public class KafkaMessageSubscriptionBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaMessageSubscriptionBuilder> _logger;
    private readonly List<SubscriptionConfiguration> _subscriptions = new();

    public KafkaMessageSubscriptionBuilder(
        IServiceProvider serviceProvider,
        ILogger<KafkaMessageSubscriptionBuilder> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Subscribe to a message type (will auto-discover handler from DI)
    /// </summary>
    public KafkaMessageSubscriptionBuilder Subscribe<TMessage>()
        where TMessage : class, IMessage
    {
        var topic = typeof(TMessage).Name.ToLowerInvariant();
        _subscriptions.Add(new SubscriptionConfiguration
        {
            MessageType = typeof(TMessage),
            Topic = topic,
            AutoDiscoverHandler = true
        });

        _logger.LogInformation("Configured subscription for {MessageType} on topic {Topic} (handler will be auto-discovered)",
            typeof(TMessage).Name, topic);

        return this;
    }

    /// <summary>
    /// Subscribe to a message type with custom topic (will auto-discover handler from DI)
    /// </summary>
    public KafkaMessageSubscriptionBuilder Subscribe<TMessage>(string topic)
        where TMessage : class, IMessage
    {
        _subscriptions.Add(new SubscriptionConfiguration
        {
            MessageType = typeof(TMessage),
            Topic = topic,
            AutoDiscoverHandler = true
        });

        _logger.LogInformation("Configured subscription for {MessageType} on topic {Topic} (handler will be auto-discovered)",
            typeof(TMessage).Name, topic);

        return this;
    }

    /// <summary>
    /// Subscribe to a message type with explicit handler
    /// </summary>
    public KafkaMessageSubscriptionBuilder Subscribe<TMessage, THandler>()
        where TMessage : class, IMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        var topic = typeof(TMessage).Name.ToLowerInvariant();
        _subscriptions.Add(new SubscriptionConfiguration
        {
            MessageType = typeof(TMessage),
            HandlerType = typeof(THandler),
            Topic = topic
        });

        _logger.LogInformation("Configured subscription for {MessageType} on topic {Topic} with handler {HandlerType}",
            typeof(TMessage).Name, topic, typeof(THandler).Name);

        return this;
    }

    /// <summary>
    /// Subscribe to a message type with explicit handler and custom topic
    /// </summary>
    public KafkaMessageSubscriptionBuilder Subscribe<TMessage, THandler>(string topic)
        where TMessage : class, IMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        _subscriptions.Add(new SubscriptionConfiguration
        {
            MessageType = typeof(TMessage),
            HandlerType = typeof(THandler),
            Topic = topic
        });

        _logger.LogInformation("Configured subscription for {MessageType} on topic {Topic} with handler {HandlerType}",
            typeof(TMessage).Name, topic, typeof(THandler).Name);

        return this;
    }

    /// <summary>
    /// Subscribe to an integration event
    /// </summary>
    public KafkaMessageSubscriptionBuilder SubscribeIntegrationEvent<TEvent, THandler>(string topic)
        where TEvent : class, IIntegrationEvent
        where THandler : class, IEventHandler<TEvent>
    {
        _subscriptions.Add(new SubscriptionConfiguration
        {
            MessageType = typeof(TEvent),
            HandlerType = typeof(THandler),
            Topic = topic,
            IsIntegrationEvent = true
        });

        _logger.LogInformation("Configured integration event subscription for {EventType} on topic {Topic} with handler {HandlerType}",
            typeof(TEvent).Name, topic, typeof(THandler).Name);

        return this;
    }

    /// <summary>
    /// Subscribe to multiple integration events with pattern-based topics
    /// </summary>
    public KafkaMessageSubscriptionBuilder SubscribeIntegrationEvents<THandler>(params string[] topicPatterns)
        where THandler : class
    {
        foreach (var pattern in topicPatterns)
        {
            _subscriptions.Add(new SubscriptionConfiguration
            {
                HandlerType = typeof(THandler),
                Topic = pattern,
                IsIntegrationEvent = true,
                IsPatternSubscription = true
            });

            _logger.LogInformation("Configured integration event subscription pattern {Pattern} with handler {HandlerType}",
                pattern, typeof(THandler).Name);
        }

        return this;
    }

    /// <summary>
    /// Build and activate all subscriptions
    /// </summary>
    public async Task BuildAsync(CancellationToken cancellationToken = default)
    {
        var messageBus = _serviceProvider.GetService<IMessageBus>();
        if (messageBus == null)
        {
            _logger.LogError("IMessageBus not registered in DI container");
            throw new InvalidOperationException("IMessageBus not registered");
        }

        _logger.LogInformation("Building {Count} Kafka subscriptions", _subscriptions.Count);

        foreach (var subscription in _subscriptions)
        {
            try
            {
                await ActivateSubscriptionAsync(subscription, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate subscription for topic {Topic}", subscription.Topic);
                throw;
            }
        }

        _logger.LogInformation("Successfully built all Kafka subscriptions");
    }

    private async Task ActivateSubscriptionAsync(SubscriptionConfiguration config, CancellationToken cancellationToken)
    {
        if (config.MessageType == null)
        {
            _logger.LogWarning("Skipping subscription with no message type for topic {Topic}", config.Topic);
            return;
        }

        // Auto-discover handler if needed
        Type handlerType;
        if (config.AutoDiscoverHandler)
        {
            handlerType = TryDiscoverHandler(config.MessageType);
            if (handlerType == null)
            {
                _logger.LogWarning("No handler found for message type {MessageType}, skipping subscription", config.MessageType.Name);
                return;
            }
        }
        else
        {
            handlerType = config.HandlerType!;
        }

        // Get the handler from DI
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            _logger.LogError("Handler {HandlerType} not registered in DI container", handlerType.Name);
            throw new InvalidOperationException($"Handler {handlerType.Name} not registered");
        }

        // Create handler delegate
        var subscribeMethod = typeof(IMessageBus).GetMethod(nameof(IMessageBus.SubscribeAsync));
        if (subscribeMethod == null)
        {
            throw new InvalidOperationException("SubscribeAsync method not found on IMessageBus");
        }

        var genericMethod = subscribeMethod.MakeGenericMethod(config.MessageType);
        var messageBus = _serviceProvider.GetRequiredService<IMessageBus>();

        // Create the handler delegate
        var handlerDelegate = CreateHandlerDelegate(config.MessageType, handlerType, handler);

        if (string.IsNullOrEmpty(config.Topic))
        {
            // Subscribe with default topic
            await (Task)genericMethod.Invoke(messageBus, new object[] { handlerDelegate, cancellationToken })!;
        }
        else
        {
            // Subscribe with custom topic
            var topicMethod = typeof(IMessageBus).GetMethods()
                .First(m => m.Name == nameof(IMessageBus.SubscribeAsync) && m.GetParameters().Length == 3)
                .MakeGenericMethod(config.MessageType);

            await (Task)topicMethod.Invoke(messageBus, new object[] { config.Topic, handlerDelegate, cancellationToken })!;
        }

        _logger.LogInformation("Activated subscription for {MessageType} on topic {Topic}",
            config.MessageType.Name, config.Topic ?? "default");
    }

    private Type? TryDiscoverHandler(Type messageType)
    {
        // Try to find IMessageHandler<TMessage>
        var handlerInterfaceType = typeof(IMessageHandler<>).MakeGenericType(messageType);
        var handlerImplementations = _serviceProvider.GetServices(handlerInterfaceType);
        
        return handlerImplementations.FirstOrDefault()?.GetType();
    }

    private static Delegate CreateHandlerDelegate(Type messageType, Type handlerType, object handler)
    {
        var handleMethod = handlerType.GetMethod("HandleAsync") ??
                          handlerType.GetMethod("Handle");

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"No HandleAsync or Handle method found on {handlerType.Name}");
        }

        // Create Func<TMessage, CancellationToken, Task>
        var delegateType = typeof(Func<,,>).MakeGenericType(messageType, typeof(CancellationToken), typeof(Task));

        return Delegate.CreateDelegate(delegateType, handler, handleMethod);
    }

    private class SubscriptionConfiguration
    {
        public Type? MessageType { get; init; }
        public Type? HandlerType { get; init; }
        public string Topic { get; init; } = string.Empty;
        public bool IsIntegrationEvent { get; init; }
        public bool IsPatternSubscription { get; init; }
        public bool AutoDiscoverHandler { get; init; }
    }
}

/// <summary>
/// Extension methods for KafkaMessageSubscriptionBuilder
/// </summary>
public static class KafkaMessageSubscriptionBuilderExtensions
{
    /// <summary>
    /// Create a new Kafka message subscription builder
    /// </summary>
    public static KafkaMessageSubscriptionBuilder CreateKafkaSubscriptionBuilder(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<KafkaMessageSubscriptionBuilder>>();
        return new KafkaMessageSubscriptionBuilder(serviceProvider, logger);
    }

    /// <summary>
    /// Configure Kafka subscriptions using a fluent builder
    /// </summary>
    public static async Task ConfigureKafkaSubscriptionsAsync(
        this IServiceProvider serviceProvider,
        Action<KafkaMessageSubscriptionBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        var builder = serviceProvider.CreateKafkaSubscriptionBuilder();
        configure(builder);
        await builder.BuildAsync(cancellationToken);
    }
}
