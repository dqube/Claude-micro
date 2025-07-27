using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactDeactivatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }

    public ContactDeactivatedEvent(ContactId contactId)
    {
        ContactId = contactId;
    }
}