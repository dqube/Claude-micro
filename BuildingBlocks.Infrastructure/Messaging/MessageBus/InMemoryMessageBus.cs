using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.MessageBus;

public partial class InMemoryMessageBus : IMessageBus
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
        ArgumentNullException.ThrowIfNull(metadata);
        
        if (!_isStarted)
        {
            LogMessageBusNotStarted(_logger);
            return;
        }

        var messageType = typeof(T);
        if (_handlers.TryGetValue(messageType, out var handlers))
        {
            var tasks = handlers.Select(handler => ExecuteHandler(handler, message, metadata));
            await Task.WhenAll(tasks);
        }

        LogMessagePublished(_logger, messageType.Name, metadata.MessageId);
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

        LogSubscribedToMessageType(_logger, messageType.Name);
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        var messageType = typeof(T);
        _handlers.TryRemove(messageType, out _);
        
        LogUnsubscribedFromMessageType(_logger, messageType.Name);
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _isStarted = true;
        LogMessageBusStarted(_logger);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _isStarted = false;
        LogMessageBusStopped(_logger);
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
            LogMessageHandlerError(_logger, ex, metadata.MessageId);
        }
        catch (TaskCanceledException ex)
        {
            LogMessageHandlerError(_logger, ex, metadata.MessageId);
        }
        catch (TimeoutException ex)
        {
            LogMessageHandlerError(_logger, ex, metadata.MessageId);
        }
        catch (ArgumentException ex)
        {
            LogMessageHandlerError(_logger, ex, metadata.MessageId);
        }
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Message bus is not started. Message will be ignored.")]
    private static partial void LogMessageBusNotStarted(ILogger logger);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Debug,
        Message = "Published message of type {messageType} with ID {messageId}")]
    private static partial void LogMessagePublished(ILogger logger, string messageType, string messageId);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Debug,
        Message = "Subscribed to message type {messageType}")]
    private static partial void LogSubscribedToMessageType(ILogger logger, string messageType);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Debug,
        Message = "Unsubscribed from message type {messageType}")]
    private static partial void LogUnsubscribedFromMessageType(ILogger logger, string messageType);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Information,
        Message = "InMemoryMessageBus started")]
    private static partial void LogMessageBusStarted(ILogger logger);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Information,
        Message = "InMemoryMessageBus stopped")]
    private static partial void LogMessageBusStopped(ILogger logger);

    [LoggerMessage(
        EventId = 7,
        Level = LogLevel.Error,
        Message = "Error executing message handler for message {messageId}")]
    private static partial void LogMessageHandlerError(ILogger logger, Exception exception, string messageId);
}