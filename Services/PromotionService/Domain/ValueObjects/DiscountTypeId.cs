using BuildingBlocks.Domain.StronglyTypedIds;

namespace PromotionService.Domain.ValueObjects;

public class DiscountTypeId : StronglyTypedId<int>
{
    public DiscountTypeId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("DiscountTypeId must be positive", nameof(value));
    }
    
    public static DiscountTypeId From(int value) => new(value);
} 