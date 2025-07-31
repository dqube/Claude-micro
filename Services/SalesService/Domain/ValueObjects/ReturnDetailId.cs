using BuildingBlocks.Domain.StronglyTypedIds;

namespace SalesService.Domain.ValueObjects;

public class ReturnDetailId : StronglyTypedId<Guid>
{
    public ReturnDetailId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ReturnDetailId cannot be empty", nameof(value));
    }
    
    public static ReturnDetailId New() => new(Guid.NewGuid());
    
    public static ReturnDetailId From(Guid value) => new(value);
}