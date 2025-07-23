using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Application.Messaging;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    Task PublishAsync<TEvent>(TEvent eventData, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;

    Task PublishIntegrationEventAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent;

    Task SubscribeAsync<TEvent, THandler>(CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>;

    Task UnsubscribeAsync<TEvent, THandler>(CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>;
}