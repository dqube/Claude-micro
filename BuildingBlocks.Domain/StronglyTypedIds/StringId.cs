namespace BuildingBlocks.Domain.StronglyTypedIds;

public abstract class StringId : StronglyTypedId<string>
{
    protected StringId(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Id cannot be null or whitespace", nameof(value));
    }
}