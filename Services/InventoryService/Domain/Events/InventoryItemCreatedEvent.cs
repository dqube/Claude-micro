using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Events;

public class InventoryItemCreatedEvent : DomainEventBase
{
    public InventoryItemId InventoryItemId { get; }
    public StoreId StoreId { get; }
    public ProductId ProductId { get; }
    public int InitialQuantity { get; }

    public InventoryItemCreatedEvent(
        InventoryItemId inventoryItemId,
        StoreId storeId,
        ProductId productId,
        int initialQuantity)
    {
        InventoryItemId = inventoryItemId;
        StoreId = storeId;
        ProductId = productId;
        InitialQuantity = initialQuantity;
    }
}