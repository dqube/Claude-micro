using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class DiscountCampaignUpdatedEvent : DomainEventBase
{
    public CampaignId CampaignId { get; }
    public string Name { get; }

    public DiscountCampaignUpdatedEvent(CampaignId campaignId, string name)
    {
        CampaignId = campaignId;
        Name = name;
    }
} 