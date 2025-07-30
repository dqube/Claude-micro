using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Queries;

public class GetInventoryByStoreQuery : QueryBase<IReadOnlyList<InventoryItemDto>>
{
    public int StoreId { get; init; }

    public GetInventoryByStoreQuery(int storeId)
    {
        StoreId = storeId;
    }
}