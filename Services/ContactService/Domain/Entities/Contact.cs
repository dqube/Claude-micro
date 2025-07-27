using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using ContactService.Domain.Events;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Entities;

public class Contact : AggregateRoot<ContactId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Address? Address { get; private set; }
    public ContactType ContactType { get; private set; }
    public string? Company { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    private Contact() 
    { 
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = null!;
        ContactType = null!;
    }

    public Contact(
        ContactId id,
        string firstName,
        string lastName,
        Email email,
        ContactType contactType,
        PhoneNumber? phoneNumber = null,
        Address? address = null,
        string? company = null,
        string? jobTitle = null,
        string? notes = null) : base(id)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ContactType = contactType ?? throw new ArgumentNullException(nameof(contactType));
        PhoneNumber = phoneNumber;
        Address = address;
        Company = company?.Trim();
        JobTitle = jobTitle?.Trim();
        Notes = notes?.Trim();
        IsActive = true;

        AddDomainEvent(new ContactCreatedEvent(Id, Email, PhoneNumber));
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateContactInformation(Email email, PhoneNumber? phoneNumber = null)
    {
        if (!Email.Equals(email) || !Equals(PhoneNumber, phoneNumber))
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber;

            AddDomainEvent(new ContactInformationUpdatedEvent(Id, Email, PhoneNumber));
        }
    }

    public void UpdateAddress(Address? address)
    {
        if (!Equals(Address, address))
        {
            Address = address;
            AddDomainEvent(new ContactAddressUpdatedEvent(Id, Address));
        }
    }

    public void UpdatePersonalInformation(string firstName, string lastName, string? company = null, string? jobTitle = null)
    {
        var newFirstName = ValidateName(firstName, nameof(firstName));
        var newLastName = ValidateName(lastName, nameof(lastName));

        if (FirstName != newFirstName || LastName != newLastName || Company != company?.Trim() || JobTitle != jobTitle?.Trim())
        {
            FirstName = newFirstName;
            LastName = newLastName;
            Company = company?.Trim();
            JobTitle = jobTitle?.Trim();

            AddDomainEvent(new ContactPersonalInformationUpdatedEvent(Id, FirstName, LastName, Company, JobTitle));
        }
    }

    public void UpdateNotes(string? notes)
    {
        var trimmedNotes = notes?.Trim();
        if (Notes != trimmedNotes)
        {
            Notes = trimmedNotes;
            AddDomainEvent(new ContactNotesUpdatedEvent(Id, Notes));
        }
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            AddDomainEvent(new ContactActivatedEvent(Id));
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            AddDomainEvent(new ContactDeactivatedEvent(Id));
        }
    }

    private static string ValidateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        
        var trimmedName = name.Trim();
        if (trimmedName.Length < 2)
            throw new ArgumentException($"{parameterName} must be at least 2 characters long", parameterName);
        
        return trimmedName;
    }
}