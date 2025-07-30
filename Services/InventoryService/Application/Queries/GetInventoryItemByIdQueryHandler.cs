using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Queries;

public class GetInventoryItemByIdQueryHandler : IQueryHandler<GetInventoryItemByIdQuery, InventoryItemDto?>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public GetInventoryItemByIdQueryHandler(IInventoryItemRepository inventoryItemRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<InventoryItemDto?> HandleAsync(GetInventoryItemByIdQuery request, CancellationToken cancellationToken = default)
    {
        var inventoryItemId = InventoryItemId.From(request.InventoryItemId);
        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(inventoryItemId, cancellationToken);

        if (inventoryItem == null)
            return null;

        return new InventoryItemDto(
            inventoryItem.Id.Value,
            inventoryItem.StoreId.Value,
            inventoryItem.ProductId.Value,
            inventoryItem.Quantity,
            inventoryItem.ReorderLevel,
            inventoryItem.LastRestockDate,
            inventoryItem.CreatedAt,
            inventoryItem.CreatedBy?.Value,
            inventoryItem.UpdatedAt,
            inventoryItem.UpdatedBy?.Value);
    }
}