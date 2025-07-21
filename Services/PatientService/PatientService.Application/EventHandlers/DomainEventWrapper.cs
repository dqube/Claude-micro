using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Domain.DomainEvents;

namespace PatientService.Application.EventHandlers;

public class DomainEventWrapper<TDomainEvent> : IEvent
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }
    
    public DomainEventWrapper(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
        Id = domainEvent.Id;
        OccurredOn = domainEvent.OccurredOn;
        EventType = domainEvent.GetType().Name;
    }

    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
}