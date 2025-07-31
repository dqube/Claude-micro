using BuildingBlocks.Domain.StronglyTypedIds;

namespace ReportingService.Domain.ValueObjects;

public class PromotionId : StronglyTypedId<Guid>
{
    public PromotionId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PromotionId cannot be empty", nameof(value));
    }
    
    public static PromotionId New() => new(Guid.NewGuid());
    
    public static PromotionId From(Guid value) => new(value);
} 