using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class InventorySnapshotUpdatedEvent : DomainEventBase
{
    public InventorySnapshotId InventorySnapshotId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }

    public InventorySnapshotUpdatedEvent(
        InventorySnapshotId inventorySnapshotId,
        int oldQuantity,
        int newQuantity)
    {
        InventorySnapshotId = inventorySnapshotId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
} 