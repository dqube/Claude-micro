namespace BuildingBlocks.Domain.StronglyTypedIds;

public abstract class IntId : StronglyTypedId<int>
{
    protected IntId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("Id must be positive", nameof(value));
    }
}