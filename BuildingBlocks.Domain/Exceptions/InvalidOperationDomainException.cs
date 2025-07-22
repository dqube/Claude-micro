namespace BuildingBlocks.Domain.Exceptions;

public class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException()
        : base("An invalid domain operation was attempted.")
    {
    }

    public InvalidOperationDomainException(string message) : base(message)
    {
    }

    public InvalidOperationDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}