using BuildingBlocks.Application.CQRS.Commands;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Commands;

public class CreateInventoryItemCommand : CommandBase<InventoryItemDto>
{
    public int StoreId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public int ReorderLevel { get; init; }
    public Guid? CreatedBy { get; init; }

    public CreateInventoryItemCommand(
        int storeId,
        Guid productId,
        int quantity = 0,
        int reorderLevel = 10,
        Guid? createdBy = null)
    {
        StoreId = storeId;
        ProductId = productId;
        Quantity = quantity;
        ReorderLevel = reorderLevel;
        CreatedBy = createdBy;
    }
}