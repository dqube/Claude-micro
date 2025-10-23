using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.InMemory;

/// <summary>
/// In-memory implementation of IMessageBus for development and testing
/// </summary>
public sealed class InMemoryMessageBus : IMessageBus, IDisposable
{
    private readonly ILogger<InMemoryMessageBus> _logger;
    private readonly ConcurrentDictionary<string, List<SubscriptionInfo>> _subscriptions = new();
    private readonly ConcurrentDictionary<Type, string> _topicCache = new();
    private bool _disposed;

    public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("InMemoryMessageBus initialized");
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishAsync(message, topic, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        _logger.LogDebug(
            "Publishing message to in-memory topic {Topic}, MessageId: {MessageId}",
            topic, message.Id);

        if (_subscriptions.TryGetValue(topic, out var subscriptions))
        {
            var tasks = subscriptions
                .Where(s => s.MessageType == typeof(T))
                .Select(s => InvokeHandler(s, message, null, cancellationToken));

            await Task.WhenAll(tasks);
        }

        _logger.LogDebug(
            "Message published to in-memory topic {Topic}, MessageId: {MessageId}",
            topic, message.Id);
    }

    public async Task PublishAsync<T>(T message, MessageEnvelop metadata, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishAsync(message, topic, metadata, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, MessageEnvelop metadata, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(metadata);

        _logger.LogDebug(
            "Publishing message with metadata to in-memory topic {Topic}, MessageId: {MessageId}, CorrelationId: {CorrelationId}",
            topic, metadata.MessageId, metadata.CorrelationId);

        if (_subscriptions.TryGetValue(topic, out var subscriptions))
        {
            var tasks = subscriptions
                .Where(s => s.MessageType == typeof(T))
                .Select(s => InvokeHandler(s, message, metadata, cancellationToken));

            await Task.WhenAll(tasks);
        }

        _logger.LogDebug(
            "Message with metadata published to in-memory topic {Topic}, MessageId: {MessageId}",
            topic, metadata.MessageId);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishBatchAsync(messages, topic, cancellationToken);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string topic, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        var messageList = messages.ToList();
        if (messageList.Count == 0)
        {
            _logger.LogWarning("PublishBatchAsync called with empty message collection");
            return;
        }

        var tasks = messageList.Select(message => PublishAsync(message, topic, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Batch of {Count} messages published to in-memory topic {Topic}",
            messageList.Count, topic);
    }

    public async Task ScheduleAsync<T>(T message, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await ScheduleAsync(message, topic, scheduledTime, cancellationToken);
    }

    public async Task ScheduleAsync<T>(T message, string topic, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        var delay = scheduledTime - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            await PublishAsync(message, topic, cancellationToken);
            return;
        }

        _logger.LogInformation(
            "Scheduling message {MessageId} for delivery at {ScheduledTime} (delay: {Delay})",
            message.Id, scheduledTime, delay);

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
                await PublishAsync(message, topic, cancellationToken);

                _logger.LogInformation(
                    "Scheduled message {MessageId} published to in-memory topic {Topic}",
                    message.Id, topic);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Scheduled message {MessageId} cancelled before delivery",
                    message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error publishing scheduled message {MessageId} to in-memory topic {Topic}",
                    message.Id, topic);
            }
        }, cancellationToken);

        await Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return SubscribeAsync(topic, handler, cancellationToken);
    }

    public Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        var subscriptionInfo = new SubscriptionInfo
        {
            Topic = topic,
            MessageType = typeof(T),
            Handler = handler,
            HandlerWithMetadata = null
        };

        _subscriptions.AddOrUpdate(
            topic,
            _ => new List<SubscriptionInfo> { subscriptionInfo },
            (_, existing) =>
            {
                existing.Add(subscriptionInfo);
                return existing;
            });

        _logger.LogInformation(
            "Subscribed to in-memory topic: {Topic} with message type: {MessageType}",
            topic, typeof(T).Name);

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return SubscribeAsync(topic, handler, cancellationToken);
    }

    public Task SubscribeAsync<T>(string topic, Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        var subscriptionInfo = new SubscriptionInfo
        {
            Topic = topic,
            MessageType = typeof(T),
            Handler = null,
            HandlerWithMetadata = handler
        };

        _subscriptions.AddOrUpdate(
            topic,
            _ => new List<SubscriptionInfo> { subscriptionInfo },
            (_, existing) =>
            {
                existing.Add(subscriptionInfo);
                return existing;
            });

        _logger.LogInformation(
            "Subscribed to in-memory topic: {Topic} with message type: {MessageType} (with metadata)",
            topic, typeof(T).Name);

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return UnsubscribeAsync(topic, cancellationToken);
    }

    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        if (_subscriptions.TryRemove(topic, out _))
        {
            _logger.LogInformation("Unsubscribed from in-memory topic: {Topic}", topic);
        }
        else
        {
            _logger.LogWarning("No active subscription found for in-memory topic {Topic}", topic);
        }

        return Task.CompletedTask;
    }

    private async Task InvokeHandler<T>(
        SubscriptionInfo subscriptionInfo,
        T message,
        MessageEnvelop? metadata,
        CancellationToken cancellationToken) where T : class, IMessage
    {
        try
        {
            if (subscriptionInfo.HandlerWithMetadata != null && metadata != null)
            {
                var typedHandler = subscriptionInfo.HandlerWithMetadata as Func<T, MessageEnvelop, CancellationToken, Task>;
                if (typedHandler != null)
                {
                    await typedHandler(message, metadata, cancellationToken);
                }
            }
            else if (subscriptionInfo.Handler != null)
            {
                var typedHandler = subscriptionInfo.Handler as Func<T, CancellationToken, Task>;
                if (typedHandler != null)
                {
                    await typedHandler(message, cancellationToken);
                }
            }

            _logger.LogDebug(
                "Successfully processed message from in-memory topic {Topic}, MessageId: {MessageId}",
                subscriptionInfo.Topic, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling message from in-memory topic {Topic}, MessageId: {MessageId}",
                subscriptionInfo.Topic, message.Id);
            throw;
        }
    }

    private string GetTopicName<T>()
    {
        return _topicCache.GetOrAdd(typeof(T), type => type.Name.ToLowerInvariant());
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _subscriptions.Clear();

        _logger.LogInformation("InMemoryMessageBus disposed");
    }

    private sealed class SubscriptionInfo
    {
        required public string Topic { get; init; }
        required public Type MessageType { get; init; }
        public object? Handler { get; init; }
        public object? HandlerWithMetadata { get; init; }
    }
}
