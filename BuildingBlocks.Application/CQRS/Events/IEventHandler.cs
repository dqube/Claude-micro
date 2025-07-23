namespace BuildingBlocks.Application.CQRS.Events;

public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
}