namespace BuildingBlocks.Domain.StronglyTypedIds;

public abstract class LongId : StronglyTypedId<long>
{
    protected LongId(long value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("Id must be positive", nameof(value));
    }
}