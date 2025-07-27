using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactAddressUpdatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }
    public Address? Address { get; }

    public ContactAddressUpdatedEvent(ContactId contactId, Address? address)
    {
        ContactId = contactId;
        Address = address;
    }
}