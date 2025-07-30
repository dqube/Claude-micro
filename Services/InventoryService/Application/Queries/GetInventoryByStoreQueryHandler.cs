using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Queries;

public class GetInventoryByStoreQueryHandler : IQueryHandler<GetInventoryByStoreQuery, IReadOnlyList<InventoryItemDto>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public GetInventoryByStoreQueryHandler(IInventoryItemRepository inventoryItemRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> HandleAsync(GetInventoryByStoreQuery request, CancellationToken cancellationToken = default)
    {
        var storeId = StoreId.From(request.StoreId);
        var inventoryItems = await _inventoryItemRepository.GetByStoreAsync(storeId, cancellationToken);

        return inventoryItems.Select(item => new InventoryItemDto(
            item.Id.Value,
            item.StoreId.Value,
            item.ProductId.Value,
            item.Quantity,
            item.ReorderLevel,
            item.LastRestockDate,
            item.CreatedAt,
            item.CreatedBy?.Value,
            item.UpdatedAt,
            item.UpdatedBy?.Value)).ToList();
    }
}