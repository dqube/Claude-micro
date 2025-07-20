using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Entities;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : class, IStronglyTypedId
{
    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}