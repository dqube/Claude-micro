using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Queries;

public class GetSalesSnapshotsByStoreQueryHandler : IQueryHandler<GetSalesSnapshotsByStoreQuery, IEnumerable<SalesSnapshotDto>>
{
    private readonly ISalesSnapshotRepository _salesSnapshotRepository;

    public GetSalesSnapshotsByStoreQueryHandler(ISalesSnapshotRepository salesSnapshotRepository)
    {
        _salesSnapshotRepository = salesSnapshotRepository;
    }

    public async Task<IEnumerable<SalesSnapshotDto>> HandleAsync(GetSalesSnapshotsByStoreQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var storeId = StoreId.From(request.StoreId);
        
        IEnumerable<SalesSnapshot> salesSnapshots;

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            salesSnapshots = await _salesSnapshotRepository.GetByStoreAndDateRangeAsync(
                storeId, request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else
        {
            salesSnapshots = await _salesSnapshotRepository.GetByStoreIdAsync(storeId, cancellationToken);
        }

        return salesSnapshots.Select(MapToDto);
    }

    private static SalesSnapshotDto MapToDto(SalesSnapshot salesSnapshot)
    {
        return new SalesSnapshotDto
        {
            Id = salesSnapshot.Id.Value,
            SaleId = salesSnapshot.SaleId.Value,
            StoreId = salesSnapshot.StoreId.Value,
            SaleDate = salesSnapshot.SaleDate,
            TotalAmount = salesSnapshot.TotalAmount,
            CustomerId = salesSnapshot.CustomerId?.Value,
            CreatedAt = salesSnapshot.CreatedAt,
            CreatedBy = salesSnapshot.CreatedBy
        };
    }
} 