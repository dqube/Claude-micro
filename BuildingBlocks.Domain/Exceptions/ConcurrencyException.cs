namespace BuildingBlocks.Domain.Exceptions;

public class ConcurrencyException : DomainException
{
    public ConcurrencyException(string message) : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ConcurrencyException() : base("A concurrency conflict occurred. The data has been modified by another user.")
    {
    }
}