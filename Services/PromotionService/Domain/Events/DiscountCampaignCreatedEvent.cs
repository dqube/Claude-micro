using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class DiscountCampaignCreatedEvent : DomainEventBase
{
    public CampaignId CampaignId { get; }
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DiscountCampaignCreatedEvent(CampaignId campaignId, string name, DateTime startDate, DateTime endDate)
    {
        CampaignId = campaignId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }
} 