using BuildingBlocks.Domain.Exceptions;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Exceptions;

public class InventoryItemNotFoundException : DomainException
{
    public InventoryItemId InventoryItemId { get; }

    public InventoryItemNotFoundException(InventoryItemId inventoryItemId)
        : base($"Inventory item with ID '{inventoryItemId}' was not found.")
    {
        InventoryItemId = inventoryItemId;
    }
}