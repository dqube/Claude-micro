using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.StronglyTypedIds;

public abstract class StronglyTypedId<T> : IStronglyTypedId, IEquatable<StronglyTypedId<T>>
    where T : notnull
{
    public T Value { get; }

    object IStronglyTypedId.Value => Value;

    protected StronglyTypedId(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool Equals(StronglyTypedId<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is StronglyTypedId<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }

    public static bool operator ==(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return !Equals(left, right);
    }
}