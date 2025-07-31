using BuildingBlocks.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Entities;

public class ReturnDetail : Entity<ReturnDetailId>, IAuditableEntity
{
    public ReturnId ReturnId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public ReturnReason Reason { get; private set; }
    public bool Restock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private ReturnDetail() : base(ReturnDetailId.New())
    {
        ReturnId = ReturnId.New();
        Reason = ReturnReason.Other;
    }

    public ReturnDetail(
        ReturnDetailId id,
        ReturnId returnId,
        Guid productId,
        int quantity,
        ReturnReason reason,
        bool restock = true) : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        ReturnId = returnId ?? throw new ArgumentNullException(nameof(returnId));
        ProductId = productId;
        Quantity = quantity;
        Reason = reason;
        Restock = restock;
        CreatedAt = DateTime.UtcNow;
    }
}