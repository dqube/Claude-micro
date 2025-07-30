using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class DiscountCampaignActivatedEvent : DomainEventBase
{
    public CampaignId CampaignId { get; }

    public DiscountCampaignActivatedEvent(CampaignId campaignId)
    {
        CampaignId = campaignId;
    }
} 