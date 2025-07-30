using BuildingBlocks.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Entities;

public class PromotionProduct : Entity<PromotionProductId>
{
    public PromotionId PromotionId { get; private set; }
    public Guid? ProductId { get; private set; }
    public int? CategoryId { get; private set; }
    public int MinQuantity { get; private set; }
    public decimal? DiscountPercent { get; private set; }
    public decimal? BundlePrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private PromotionProduct() : base(PromotionProductId.New())
    {
        PromotionId = PromotionId.New();
        MinQuantity = 1;
    }

    public PromotionProduct(
        PromotionProductId id,
        PromotionId promotionId,
        int minQuantity = 1) : base(id)
    {
        if (minQuantity < 1)
            throw new ArgumentException("Minimum quantity must be at least 1", nameof(minQuantity));

        PromotionId = promotionId;
        MinQuantity = minQuantity;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetProductTarget(Guid productId, Guid updatedBy)
    {
        ProductId = productId;
        CategoryId = null;
        CreatedBy = updatedBy;
    }

    public void SetCategoryTarget(int categoryId, Guid updatedBy)
    {
        CategoryId = categoryId;
        ProductId = null;
        CreatedBy = updatedBy;
    }

    public void SetDiscountPercent(decimal discountPercent, Guid updatedBy)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("Discount percent must be between 0 and 100", nameof(discountPercent));

        DiscountPercent = discountPercent;
        BundlePrice = null; // Clear bundle price when setting discount percent
        CreatedBy = updatedBy;
    }

    public void SetBundlePrice(decimal bundlePrice, Guid updatedBy)
    {
        if (bundlePrice < 0)
            throw new ArgumentException("Bundle price cannot be negative", nameof(bundlePrice));

        BundlePrice = bundlePrice;
        DiscountPercent = null; // Clear discount percent when setting bundle price
        CreatedBy = updatedBy;
    }

    public void UpdateMinQuantity(int minQuantity, Guid updatedBy)
    {
        if (minQuantity < 1)
            throw new ArgumentException("Minimum quantity must be at least 1", nameof(minQuantity));

        MinQuantity = minQuantity;
        CreatedBy = updatedBy;
    }
} 