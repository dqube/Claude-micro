using BuildingBlocks.Domain.StronglyTypedIds;

namespace PromotionService.Domain.ValueObjects;

public class PromotionProductId : StronglyTypedId<Guid>
{
    public PromotionProductId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PromotionProductId cannot be empty", nameof(value));
    }
    
    public static PromotionProductId New() => new(Guid.NewGuid());
    
    public static PromotionProductId From(Guid value) => new(value);
} 