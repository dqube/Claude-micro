using BuildingBlocks.Domain.StronglyTypedIds;

namespace SalesService.Domain.ValueObjects;

public class SaleDetailId : StronglyTypedId<Guid>
{
    public SaleDetailId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SaleDetailId cannot be empty", nameof(value));
    }
    
    public static SaleDetailId New() => new(Guid.NewGuid());
    
    public static SaleDetailId From(Guid value) => new(value);
}