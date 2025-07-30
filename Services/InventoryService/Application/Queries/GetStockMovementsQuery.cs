using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Queries;

public class GetStockMovementsQuery : QueryBase<IReadOnlyList<StockMovementDto>>
{
    public Guid? ProductId { get; init; }
    public int? StoreId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? MovementType { get; init; }

    public GetStockMovementsQuery(
        Guid? productId = null,
        int? storeId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? movementType = null)
    {
        ProductId = productId;
        StoreId = storeId;
        StartDate = startDate;
        EndDate = endDate;
        MovementType = movementType;
    }
}