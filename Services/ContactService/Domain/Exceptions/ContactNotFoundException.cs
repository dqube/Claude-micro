using BuildingBlocks.Domain.Exceptions;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Exceptions;

public class ContactNotFoundException : DomainException
{
    public ContactNotFoundException() : base("Contact was not found.")
    {
    }

    public ContactNotFoundException(string message) : base(message)
    {
    }

    public ContactNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ContactNotFoundException(ContactId contactId) 
        : base($"Contact with ID '{contactId?.Value}' was not found.")
    {
        ArgumentNullException.ThrowIfNull(contactId);
    }

    public ContactNotFoundException(string email, bool isEmail) 
        : base($"Contact with email '{email}' was not found.")
    {
    }
}