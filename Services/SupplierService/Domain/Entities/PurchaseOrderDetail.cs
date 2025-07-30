using BuildingBlocks.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Entities;

public class PurchaseOrderDetail : Entity<OrderDetailId>
{
    public OrderId OrderId { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public int? ReceivedQuantity { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    public decimal LineTotal => Quantity * UnitCost;
    public int PendingQuantity => Quantity - (ReceivedQuantity ?? 0);
    public bool IsFullyReceived => ReceivedQuantity.HasValue && ReceivedQuantity >= Quantity;

    private PurchaseOrderDetail() { } // EF Core

    public PurchaseOrderDetail(
        OrderDetailId id,
        OrderId orderId,
        Guid productId,
        int quantity,
        decimal unitCost,
        Guid? createdBy = null) : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative", nameof(unitCost));

        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
        ReceivedQuantity = null;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdateQuantityAndCost(int quantity, decimal unitCost, Guid updatedBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative", nameof(unitCost));

        Quantity = quantity;
        UnitCost = unitCost;
    }

    public void ReceiveQuantity(int receivedQuantity, Guid updatedBy)
    {
        if (receivedQuantity < 0)
            throw new ArgumentException("Received quantity cannot be negative", nameof(receivedQuantity));
        if (receivedQuantity > Quantity)
            throw new ArgumentException("Received quantity cannot exceed ordered quantity", nameof(receivedQuantity));

        ReceivedQuantity = receivedQuantity;
    }
} 