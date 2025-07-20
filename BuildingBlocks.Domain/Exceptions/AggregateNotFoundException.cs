namespace BuildingBlocks.Domain.Exceptions;

public class AggregateNotFoundException : DomainException
{
    public AggregateNotFoundException(string aggregateType, object id)
        : base($"{aggregateType} with id '{id}' was not found.")
    {
    }

    public AggregateNotFoundException(Type aggregateType, object id)
        : base($"{aggregateType.Name} with id '{id}' was not found.")
    {
    }
}