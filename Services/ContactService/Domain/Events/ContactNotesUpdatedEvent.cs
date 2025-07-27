using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactNotesUpdatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }
    public string? Notes { get; }

    public ContactNotesUpdatedEvent(ContactId contactId, string? notes)
    {
        ContactId = contactId;
        Notes = notes;
    }
}