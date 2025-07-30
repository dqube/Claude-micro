using BuildingBlocks.Domain.StronglyTypedIds;

namespace PromotionService.Domain.ValueObjects;

public class CampaignId : StronglyTypedId<Guid>
{
    public CampaignId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CampaignId cannot be empty", nameof(value));
    }
    
    public static CampaignId New() => new(Guid.NewGuid());
    
    public static CampaignId From(Guid value) => new(value);
} 