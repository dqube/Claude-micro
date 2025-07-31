using BuildingBlocks.Domain.StronglyTypedIds;

namespace SalesService.Domain.ValueObjects;

public class AppliedDiscountId : StronglyTypedId<Guid>
{
    public AppliedDiscountId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AppliedDiscountId cannot be empty", nameof(value));
    }
    
    public static AppliedDiscountId New() => new(Guid.NewGuid());
    
    public static AppliedDiscountId From(Guid value) => new(value);
}