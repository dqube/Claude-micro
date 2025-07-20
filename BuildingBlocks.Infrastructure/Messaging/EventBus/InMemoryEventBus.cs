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
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        var eventType = typeof(T);
        _logger.LogDebug("Publishing event {EventType}", eventType.Name);

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
        
        _logger.LogDebug("Subscribed to event {EventType}", eventType.Name);
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>() where T : IEvent
    {
        var eventType = typeof(T);
        _handlers.TryRemove(eventType, out _);
        _logger.LogDebug("Unsubscribed from event {EventType}", eventType.Name);
        return Task.CompletedTask;
    }
}