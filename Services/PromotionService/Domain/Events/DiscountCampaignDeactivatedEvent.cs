using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class DiscountCampaignDeactivatedEvent : DomainEventBase
{
    public CampaignId CampaignId { get; }

    public DiscountCampaignDeactivatedEvent(CampaignId campaignId)
    {
        CampaignId = campaignId;
    }
} 