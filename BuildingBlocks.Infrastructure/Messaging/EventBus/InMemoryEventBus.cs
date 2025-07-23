using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Infrastructure.Messaging.EventBus;

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, CancellationToken, Task>>> _handlers = new();
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var eventType = typeof(T);
        LogPublishing(_logger, eventType.Name, null);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var tasks = handlers.Select(handler => handler(@event!, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }

    public async Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IEvent
    {
        var tasks = events.Select(e => PublishAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler) where T : IEvent
    {
        var eventType = typeof(T);
        _handlers.AddOrUpdate(eventType,
            _ => new List<Func<object, CancellationToken, Task>> { (e, ct) => handler((T)e, ct) },
            (_, existing) => { existing.Add((e, ct) => handler((T)e, ct)); return existing; });
        
        LogSubscribed(_logger, eventType.Name, null);
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>() where T : IEvent
    {
        var eventType = typeof(T);
        _handlers.TryRemove(eventType, out _);
        LogUnsubscribed(_logger, eventType.Name, null);
        return Task.CompletedTask;
    }

    private static readonly Action<ILogger, string, Exception?> LogPublishing =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2001, "Publishing"), "Publishing event {EventType}");

    private static readonly Action<ILogger, string, Exception?> LogSubscribed =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2002, "Subscribed"), "Subscribed to event {EventType}");

    private static readonly Action<ILogger, string, Exception?> LogUnsubscribed =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2003, "Unsubscribed"), "Unsubscribed from event {EventType}");
}