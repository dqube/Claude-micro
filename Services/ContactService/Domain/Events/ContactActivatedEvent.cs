using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactActivatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }

    public ContactActivatedEvent(ContactId contactId)
    {
        ContactId = contactId;
    }
}