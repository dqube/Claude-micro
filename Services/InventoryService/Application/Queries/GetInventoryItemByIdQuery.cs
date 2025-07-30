using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Queries;

public class GetInventoryItemByIdQuery : QueryBase<InventoryItemDto?>
{
    public Guid InventoryItemId { get; init; }

    public GetInventoryItemByIdQuery(Guid inventoryItemId)
    {
        InventoryItemId = inventoryItemId;
    }
}