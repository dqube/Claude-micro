using BuildingBlocks.Application.CQRS.Commands;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Commands;

public class CreateDiscountRuleCommand : CommandBase<DiscountRuleDto>
{
    public Guid CampaignId { get; init; }
    public string RuleType { get; init; } = string.Empty;
    public Guid? ProductId { get; init; }
    public int? CategoryId { get; init; }
    public int? MinQuantity { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal DiscountValue { get; init; }
    public string DiscountMethod { get; init; } = string.Empty;
    public Guid? FreeProductId { get; init; }

    public CreateDiscountRuleCommand(Guid campaignId, string ruleType, decimal discountValue, string discountMethod, 
        Guid? productId = null, int? categoryId = null, int? minQuantity = null, decimal? minAmount = null, Guid? freeProductId = null)
    {
        CampaignId = campaignId;
        RuleType = ruleType;
        ProductId = productId;
        CategoryId = categoryId;
        MinQuantity = minQuantity;
        MinAmount = minAmount;
        DiscountValue = discountValue;
        DiscountMethod = discountMethod;
        FreeProductId = freeProductId;
    }
} 