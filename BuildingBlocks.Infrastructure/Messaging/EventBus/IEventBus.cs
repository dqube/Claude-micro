using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Infrastructure.Messaging.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
    Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IEvent;
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler) where T : IEvent;
    Task UnsubscribeAsync<T>() where T : IEvent;
}