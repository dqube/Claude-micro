using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Queries;

public class GetSalesSnapshotByIdQueryHandler : IQueryHandler<GetSalesSnapshotByIdQuery, SalesSnapshotDto>
{
    private readonly ISalesSnapshotRepository _salesSnapshotRepository;

    public GetSalesSnapshotByIdQueryHandler(ISalesSnapshotRepository salesSnapshotRepository)
    {
        _salesSnapshotRepository = salesSnapshotRepository;
    }

    public async Task<SalesSnapshotDto> HandleAsync(GetSalesSnapshotByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var salesSnapshotId = SalesSnapshotId.From(request.SalesSnapshotId);
        var salesSnapshot = await _salesSnapshotRepository.GetByIdAsync(salesSnapshotId, cancellationToken)
            ?? throw new InvalidOperationException($"Sales snapshot with ID {request.SalesSnapshotId} not found");

        return MapToDto(salesSnapshot);
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