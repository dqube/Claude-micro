using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.MessageBus;

public class InMemoryMessageBus : IMessageBus
{
    private readonly ILogger<InMemoryMessageBus> _logger;
    private readonly ConcurrentDictionary<Type, List<Func<object, MessageMetadata, Task>>> _handlers = new();
    private bool _isStarted;

    public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var metadata = new MessageMetadata
        {
            MessageType = typeof(T).Name,
            Source = "InMemoryMessageBus"
        };
        await PublishAsync(message, metadata, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string destination, CancellationToken cancellationToken = default) where T : class
    {
        var metadata = new MessageMetadata
        {
            MessageType = typeof(T).Name,
            Source = "InMemoryMessageBus",
            Destination = destination
        };
        await PublishAsync(message, metadata, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, MessageMetadata metadata, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isStarted)
        {
            _logger.LogWarning("Message bus is not started. Message will be ignored.");
            return;
        }

        var messageType = typeof(T);
        if (_handlers.TryGetValue(messageType, out var handlers))
        {
            var tasks = handlers.Select(handler => ExecuteHandler(handler, message, metadata));
            await Task.WhenAll(tasks);
        }

        _logger.LogDebug("Published message of type {MessageType} with ID {MessageId}", 
            messageType.Name, metadata.MessageId);
    }

    public Task SubscribeAsync<T>(Func<T, MessageMetadata, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        var messageType = typeof(T);
        var wrappedHandler = new Func<object, MessageMetadata, Task>((msg, metadata) => handler((T)msg, metadata));
        
        _handlers.AddOrUpdate(messageType,
            new List<Func<object, MessageMetadata, Task>> { wrappedHandler },
            (key, existing) =>
            {
                existing.Add(wrappedHandler);
                return existing;
            });

        _logger.LogDebug("Subscribed to message type {MessageType}", messageType.Name);
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        var messageType = typeof(T);
        _handlers.TryRemove(messageType, out _);
        
        _logger.LogDebug("Unsubscribed from message type {MessageType}", messageType.Name);
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _isStarted = true;
        _logger.LogInformation("InMemoryMessageBus started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _isStarted = false;
        _logger.LogInformation("InMemoryMessageBus stopped");
        return Task.CompletedTask;
    }

    private async Task ExecuteHandler(Func<object, MessageMetadata, Task> handler, object message, MessageMetadata metadata)
    {
        try
        {
            await handler(message, metadata);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error executing message handler for message {MessageId}", metadata.MessageId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Error executing message handler for message {MessageId}", metadata.MessageId);
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Error executing message handler for message {MessageId}", metadata.MessageId);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error executing message handler for message {MessageId}", metadata.MessageId);
        }
    }
}