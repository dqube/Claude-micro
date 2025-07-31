using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class InventorySnapshotCreatedEvent : DomainEventBase
{
    public InventorySnapshotId InventorySnapshotId { get; }
    public ProductId ProductId { get; }
    public StoreId StoreId { get; }
    public int Quantity { get; }
    public DateOnly SnapshotDate { get; }

    public InventorySnapshotCreatedEvent(
        InventorySnapshotId inventorySnapshotId,
        ProductId productId,
        StoreId storeId,
        int quantity,
        DateOnly snapshotDate)
    {
        InventorySnapshotId = inventorySnapshotId;
        ProductId = productId;
        StoreId = storeId;
        Quantity = quantity;
        SnapshotDate = snapshotDate;
    }
} 