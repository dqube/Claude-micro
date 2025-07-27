using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Events;

public class ContactPersonalInformationUpdatedEvent : DomainEventBase
{
    public ContactId ContactId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? Company { get; }
    public string? JobTitle { get; }

    public ContactPersonalInformationUpdatedEvent(ContactId contactId, string firstName, string lastName, string? company, string? jobTitle)
    {
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        JobTitle = jobTitle;
    }
}