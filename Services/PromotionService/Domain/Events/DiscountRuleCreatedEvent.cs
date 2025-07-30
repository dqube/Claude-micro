using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class DiscountRuleCreatedEvent : DomainEventBase
{
    public RuleId RuleId { get; }
    public CampaignId CampaignId { get; }
    public RuleType RuleType { get; }
    public decimal DiscountValue { get; }
    public DiscountMethod DiscountMethod { get; }

    public DiscountRuleCreatedEvent(RuleId ruleId, CampaignId campaignId, RuleType ruleType, decimal discountValue, DiscountMethod discountMethod)
    {
        RuleId = ruleId;
        CampaignId = campaignId;
        RuleType = ruleType;
        DiscountValue = discountValue;
        DiscountMethod = discountMethod;
    }
} 