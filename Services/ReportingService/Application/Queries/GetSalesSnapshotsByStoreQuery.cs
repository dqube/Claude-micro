using BuildingBlocks.Application.CQRS.Queries;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Queries;

public class GetSalesSnapshotsByStoreQuery : QueryBase<IEnumerable<SalesSnapshotDto>>
{
    public int StoreId { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    public GetSalesSnapshotsByStoreQuery(int storeId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        StoreId = storeId;
        StartDate = startDate;
        EndDate = endDate;
    }
} 