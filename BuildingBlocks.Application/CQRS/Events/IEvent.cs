namespace BuildingBlocks.Application.CQRS.Events;

public interface IEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}