using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Commands;

public class CreateSalesSnapshotCommandHandler : ICommandHandler<CreateSalesSnapshotCommand, SalesSnapshotDto>
{
    private readonly ISalesSnapshotRepository _salesSnapshotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalesSnapshotCommandHandler(
        ISalesSnapshotRepository salesSnapshotRepository,
        IUnitOfWork unitOfWork)
    {
        _salesSnapshotRepository = salesSnapshotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SalesSnapshotDto> HandleAsync(CreateSalesSnapshotCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create value objects
        var saleId = SaleId.From(request.SaleId);
        var storeId = StoreId.From(request.StoreId);
        var customerId = request.CustomerId.HasValue ? CustomerId.From(request.CustomerId.Value) : null;

        // Create entity
        var salesSnapshot = new SalesSnapshot(
            SalesSnapshotId.New(),
            saleId,
            storeId,
            request.SaleDate,
            request.TotalAmount,
            customerId);

        // Add to repository
        await _salesSnapshotRepository.AddAsync(salesSnapshot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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