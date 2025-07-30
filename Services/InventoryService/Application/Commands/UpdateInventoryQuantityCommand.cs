using BuildingBlocks.Application.CQRS.Commands;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Commands;

public class UpdateInventoryQuantityCommand : CommandBase<InventoryItemDto>
{
    public Guid InventoryItemId { get; init; }
    public int NewQuantity { get; init; }
    public Guid UpdatedBy { get; init; }

    public UpdateInventoryQuantityCommand(
        Guid inventoryItemId,
        int newQuantity,
        Guid updatedBy)
    {
        InventoryItemId = inventoryItemId;
        NewQuantity = newQuantity;
        UpdatedBy = updatedBy;
    }
}