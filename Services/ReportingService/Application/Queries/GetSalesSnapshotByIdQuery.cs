using BuildingBlocks.Application.CQRS.Queries;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Queries;

public class GetSalesSnapshotByIdQuery : QueryBase<SalesSnapshotDto>
{
    public Guid SalesSnapshotId { get; init; }

    public GetSalesSnapshotByIdQuery(Guid salesSnapshotId)
    {
        SalesSnapshotId = salesSnapshotId;
    }
} 