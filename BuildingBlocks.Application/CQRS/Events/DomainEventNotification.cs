using BuildingBlocks.Domain.DomainEvents;

namespace BuildingBlocks.Application.CQRS.Events;

public class DomainEventNotification<TDomainEvent> : IEvent
    where TDomainEvent : IDomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = typeof(TDomainEvent).Name;
    }

    public TDomainEvent DomainEvent { get; }
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}