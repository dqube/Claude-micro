using BuildingBlocks.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Entities;

public class AppliedDiscount : Entity<AppliedDiscountId>, IAuditableEntity
{
    public SaleDetailId? SaleDetailId { get; private set; }
    public SaleId? SaleId { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid RuleId { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private AppliedDiscount() : base(AppliedDiscountId.New())
    {
    }

    public AppliedDiscount(
        AppliedDiscountId id,
        SaleDetailId? saleDetailId,
        SaleId? saleId,
        Guid campaignId,
        Guid ruleId,
        decimal discountAmount) : base(id)
    {
        if (saleDetailId == null && saleId == null)
            throw new ArgumentException("Either SaleDetailId or SaleId must be provided");
        
        if (saleDetailId != null && saleId != null)
            throw new ArgumentException("Cannot specify both SaleDetailId and SaleId");

        if (discountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

        SaleDetailId = saleDetailId;
        SaleId = saleId;
        CampaignId = campaignId;
        RuleId = ruleId;
        DiscountAmount = discountAmount;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsLineItemDiscount => SaleDetailId != null;
    public bool IsSaleDiscount => SaleId != null;
}