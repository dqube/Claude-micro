namespace BuildingBlocks.Domain.Exceptions;

public class AggregateNotFoundException : DomainException
{
    public AggregateNotFoundException()
        : base("Aggregate was not found.")
    {
    }

    public AggregateNotFoundException(string message)
        : base(message)
    {
    }

    public AggregateNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public AggregateNotFoundException(string aggregateType, object id)
        : base($"{aggregateType} with id '{id}' was not found.")
    {
    }

    public AggregateNotFoundException(Type aggregateType, object id)
        : base($"{aggregateType?.Name ?? "Unknown"} with id '{id}' was not found.")
    {
        ArgumentNullException.ThrowIfNull(aggregateType);
    }
}