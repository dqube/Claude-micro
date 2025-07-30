using BuildingBlocks.Domain.StronglyTypedIds;

namespace PromotionService.Domain.ValueObjects;

public class RuleId : StronglyTypedId<Guid>
{
    public RuleId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RuleId cannot be empty", nameof(value));
    }
    
    public static RuleId New() => new(Guid.NewGuid());
    
    public static RuleId From(Guid value) => new(value);
} 