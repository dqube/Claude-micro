using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Entities;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : class, IStronglyTypedId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}