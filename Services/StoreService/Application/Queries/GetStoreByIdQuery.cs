using BuildingBlocks.Application.CQRS.Queries;
using StoreService.Application.DTOs;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Queries;

public class GetStoreByIdQuery : QueryBase<StoreDto?>
{
    public StoreId StoreId { get; init; }

    public GetStoreByIdQuery(StoreId storeId)
    {
        StoreId = storeId;
    }
} 