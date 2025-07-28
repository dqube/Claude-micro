using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class UpdateStoreStatusCommand : CommandBase
{
    public StoreId StoreId { get; init; }
    public string Status { get; init; }

    public UpdateStoreStatusCommand(StoreId storeId, string status)
    {
        StoreId = storeId;
        Status = status;
    }
} 