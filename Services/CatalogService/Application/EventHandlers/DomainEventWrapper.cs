using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Domain.DomainEvents;

namespace CatalogService.Application.EventHandlers;

public class DomainEventWrapper<TDomainEvent> : IEvent
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }

    public DomainEventWrapper(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
        Id = domainEvent.Id;
        OccurredOn = domainEvent.OccurredOn;
        EventType = typeof(TDomainEvent).Name;
    }
}