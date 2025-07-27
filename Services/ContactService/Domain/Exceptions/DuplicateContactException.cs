using BuildingBlocks.Domain.Exceptions;

namespace ContactService.Domain.Exceptions;

public class DuplicateContactException : DomainException
{
    public DuplicateContactException() : base("A duplicate contact already exists.")
    {
    }

    public DuplicateContactException(string message) : base(message)
    {
    }

    public DuplicateContactException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DuplicateContactException(string email, bool isEmail) 
        : base($"A contact with email '{email}' already exists.")
    {
    }
}