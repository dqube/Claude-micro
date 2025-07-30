using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Events;

public class LowStockDetectedEvent : DomainEventBase
{
    public InventoryItemId InventoryItemId { get; }
    public StoreId StoreId { get; }
    public ProductId ProductId { get; }
    public int CurrentQuantity { get; }
    public int ReorderLevel { get; }

    public LowStockDetectedEvent(
        InventoryItemId inventoryItemId,
        StoreId storeId,
        ProductId productId,
        int currentQuantity,
        int reorderLevel)
    {
        InventoryItemId = inventoryItemId;
        StoreId = storeId;
        ProductId = productId;
        CurrentQuantity = currentQuantity;
        ReorderLevel = reorderLevel;
    }
}