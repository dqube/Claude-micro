using BuildingBlocks.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Events;

namespace PromotionService.Domain.Entities;

public class DiscountRule : AggregateRoot<RuleId>
{
    public CampaignId CampaignId { get; private set; }
    public RuleType RuleType { get; private set; }
    public Guid? ProductId { get; private set; }
    public int? CategoryId { get; private set; }
    public int? MinQuantity { get; private set; }
    public decimal? MinAmount { get; private set; }
    public decimal DiscountValue { get; private set; }
    public DiscountMethod DiscountMethod { get; private set; }
    public Guid? FreeProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private DiscountRule() : base(RuleId.New())
    {
        CampaignId = CampaignId.New();
        RuleType = RuleType.Product;
        DiscountMethod = DiscountMethod.Percent;
    }

    public DiscountRule(
        RuleId id,
        CampaignId campaignId,
        RuleType ruleType,
        decimal discountValue,
        DiscountMethod discountMethod) : base(id)
    {
        if (discountValue < 0)
            throw new ArgumentException("Discount value cannot be negative", nameof(discountValue));

        CampaignId = campaignId;
        RuleType = ruleType;
        DiscountValue = discountValue;
        DiscountMethod = discountMethod;
        CreatedAt = DateTime.UtcNow;
    }

    public DiscountRule(
        RuleId id,
        CampaignId campaignId,
        RuleType ruleType,
        Guid? productId,
        int? categoryId,
        int? minQuantity,
        decimal? minAmount,
        decimal discountValue,
        DiscountMethod discountMethod,
        Guid? freeProductId) : base(id)
    {
        if (discountValue < 0)
            throw new ArgumentException("Discount value cannot be negative", nameof(discountValue));

        if (minQuantity.HasValue && minQuantity.Value < 0)
            throw new ArgumentException("Minimum quantity cannot be negative", nameof(minQuantity));
        
        if (minAmount.HasValue && minAmount.Value < 0)
            throw new ArgumentException("Minimum amount cannot be negative", nameof(minAmount));

        CampaignId = campaignId;
        RuleType = ruleType;
        ProductId = productId;
        CategoryId = categoryId;
        MinQuantity = minQuantity;
        MinAmount = minAmount;
        DiscountValue = discountValue;
        DiscountMethod = discountMethod;
        FreeProductId = freeProductId;
        CreatedAt = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new DiscountRuleCreatedEvent(id, campaignId, ruleType, discountValue, discountMethod));
    }

    public void SetProductTarget(Guid productId, Guid updatedBy)
    {
        ProductId = productId;
        CategoryId = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetCategoryTarget(int categoryId, Guid updatedBy)
    {
        CategoryId = categoryId;
        ProductId = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetMinimumRequirements(int? minQuantity, decimal? minAmount, Guid updatedBy)
    {
        if (minQuantity.HasValue && minQuantity.Value < 0)
            throw new ArgumentException("Minimum quantity cannot be negative", nameof(minQuantity));
        
        if (minAmount.HasValue && minAmount.Value < 0)
            throw new ArgumentException("Minimum amount cannot be negative", nameof(minAmount));

        MinQuantity = minQuantity;
        MinAmount = minAmount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetFreeProduct(Guid freeProductId, Guid updatedBy)
    {
        FreeProductId = freeProductId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateDiscount(decimal discountValue, DiscountMethod discountMethod, Guid updatedBy)
    {
        if (discountValue < 0)
            throw new ArgumentException("Discount value cannot be negative", nameof(discountValue));

        DiscountValue = discountValue;
        DiscountMethod = discountMethod;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
} 