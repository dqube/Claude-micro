using BuildingBlocks.Domain.StronglyTypedIds;

namespace ReportingService.Domain.ValueObjects;

public class PromotionEffectivenessId : StronglyTypedId<Guid>
{
    public PromotionEffectivenessId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PromotionEffectivenessId cannot be empty", nameof(value));
    }
    
    public static PromotionEffectivenessId New() => new(Guid.NewGuid());
    
    public static PromotionEffectivenessId From(Guid value) => new(value);
} 