namespace BuildingBlocks.Domain.ValueObjects;

public abstract class SingleValueObject<T> : ValueObject
    where T : notnull
{
    protected SingleValueObject(T value)
    {
        Value = value;
    }

    public T Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString() ?? string.Empty;

    public static implicit operator T(SingleValueObject<T> valueObject) => valueObject.Value;
}