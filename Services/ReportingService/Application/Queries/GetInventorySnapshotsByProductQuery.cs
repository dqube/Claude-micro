using BuildingBlocks.Application.CQRS.Queries;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Queries;

public class GetInventorySnapshotsByProductQuery : QueryBase<IEnumerable<InventorySnapshotDto>>
{
    public Guid ProductId { get; init; }
    public int? StoreId { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    public GetInventorySnapshotsByProductQuery(
        Guid productId, 
        int? storeId = null, 
        DateOnly? startDate = null, 
        DateOnly? endDate = null)
    {
        ProductId = productId;
        StoreId = storeId;
        StartDate = startDate;
        EndDate = endDate;
    }
} 