namespace BuildingBlocks.Domain.StronglyTypedIds;

public abstract class GuidId : StronglyTypedId<Guid>
{
    protected GuidId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(value));
    }

    protected GuidId() : this(Guid.NewGuid())
    {
    }
}