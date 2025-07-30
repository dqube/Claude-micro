using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Events;

public class InventoryRestockedEvent : DomainEventBase
{
    public InventoryItemId InventoryItemId { get; }
    public StoreId StoreId { get; }
    public ProductId ProductId { get; }
    public DateTime RestockDate { get; }

    public InventoryRestockedEvent(
        InventoryItemId inventoryItemId,
        StoreId storeId,
        ProductId productId,
        DateTime restockDate)
    {
        InventoryItemId = inventoryItemId;
        StoreId = storeId;
        ProductId = productId;
        RestockDate = restockDate;
    }
}