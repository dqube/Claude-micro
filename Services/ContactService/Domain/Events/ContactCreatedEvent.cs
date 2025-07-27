using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactCreatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }
    public Email Email { get; }
    public PhoneNumber? PhoneNumber { get; }

    public ContactCreatedEvent(ContactId contactId, Email email, PhoneNumber? phoneNumber)
    {
        ContactId = contactId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}