using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Application.Dispatchers;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    Task DispatchAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}