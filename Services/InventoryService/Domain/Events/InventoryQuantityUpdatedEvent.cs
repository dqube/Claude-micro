using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Events;

public class InventoryQuantityUpdatedEvent : DomainEventBase
{
    public InventoryItemId InventoryItemId { get; }
    public StoreId StoreId { get; }
    public ProductId ProductId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }

    public InventoryQuantityUpdatedEvent(
        InventoryItemId inventoryItemId,
        StoreId storeId,
        ProductId productId,
        int oldQuantity,
        int newQuantity)
    {
        InventoryItemId = inventoryItemId;
        StoreId = storeId;
        ProductId = productId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
}