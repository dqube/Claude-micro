using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class DiscountAppliedEvent : DomainEventBase
{
    public SaleId SaleId { get; }
    public Guid CampaignId { get; }
    public Guid RuleId { get; }
    public decimal DiscountAmount { get; }

    public DiscountAppliedEvent(SaleId saleId, Guid campaignId, Guid ruleId, decimal discountAmount)
    {
        SaleId = saleId;
        CampaignId = campaignId;
        RuleId = ruleId;
        DiscountAmount = discountAmount;
    }
}