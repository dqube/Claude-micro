using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Dispatchers;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
        
        var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task DispatchAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        var tasks = events.Select(@event => DispatchAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }
}