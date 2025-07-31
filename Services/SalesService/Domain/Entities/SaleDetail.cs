using BuildingBlocks.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Entities;

public class SaleDetail : Entity<SaleDetailId>, IAuditableEntity
{
    public SaleId SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal AppliedDiscount { get; private set; }
    public decimal TaxApplied { get; private set; }
    public decimal LineTotal { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private SaleDetail() : base(SaleDetailId.New())
    {
        SaleId = SaleId.New();
    }

    public SaleDetail(
        SaleDetailId id,
        SaleId saleId,
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal taxApplied = 0) : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (taxApplied < 0)
            throw new ArgumentException("Tax applied cannot be negative", nameof(taxApplied));

        SaleId = saleId ?? throw new ArgumentNullException(nameof(saleId));
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxApplied = taxApplied;
        AppliedDiscount = 0;
        CreatedAt = DateTime.UtcNow;
        
        CalculateLineTotal();
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));
        
        if (discountAmount > (UnitPrice * Quantity))
            throw new ArgumentException("Discount cannot exceed line total", nameof(discountAmount));

        AppliedDiscount = discountAmount;
        CalculateLineTotal();
    }

    private void CalculateLineTotal()
    {
        LineTotal = (UnitPrice * Quantity) - AppliedDiscount + TaxApplied;
        
        if (LineTotal < 0)
            LineTotal = 0;
    }
}