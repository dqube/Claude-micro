using BuildingBlocks.Application.CQRS.Commands;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Commands;

public class CreateInventorySnapshotCommand : CommandBase<InventorySnapshotDto>
{
    public Guid ProductId { get; init; }
    public int StoreId { get; init; }
    public int Quantity { get; init; }
    public DateOnly? SnapshotDate { get; init; }

    public CreateInventorySnapshotCommand(
        Guid productId,
        int storeId,
        int quantity,
        DateOnly? snapshotDate = null)
    {
        ProductId = productId;
        StoreId = storeId;
        Quantity = quantity;
        SnapshotDate = snapshotDate;
    }
} 