using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Queries;

public class GetInventorySnapshotsByProductQueryHandler : IQueryHandler<GetInventorySnapshotsByProductQuery, IEnumerable<InventorySnapshotDto>>
{
    private readonly IInventorySnapshotRepository _inventorySnapshotRepository;

    public GetInventorySnapshotsByProductQueryHandler(IInventorySnapshotRepository inventorySnapshotRepository)
    {
        _inventorySnapshotRepository = inventorySnapshotRepository;
    }

    public async Task<IEnumerable<InventorySnapshotDto>> HandleAsync(GetInventorySnapshotsByProductQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var productId = ProductId.From(request.ProductId);
        
        IEnumerable<InventorySnapshot> inventorySnapshots;

        if (request.StoreId.HasValue && request.StartDate.HasValue && request.EndDate.HasValue)
        {
            var storeId = StoreId.From(request.StoreId.Value);
            inventorySnapshots = await _inventorySnapshotRepository.GetByProductStoreAndDateRangeAsync(
                productId, storeId, request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else
        {
            inventorySnapshots = await _inventorySnapshotRepository.GetByProductIdAsync(productId, cancellationToken);
        }

        return inventorySnapshots.Select(MapToDto);
    }

    private static InventorySnapshotDto MapToDto(InventorySnapshot inventorySnapshot)
    {
        return new InventorySnapshotDto
        {
            Id = inventorySnapshot.Id.Value,
            ProductId = inventorySnapshot.ProductId.Value,
            StoreId = inventorySnapshot.StoreId.Value,
            Quantity = inventorySnapshot.Quantity,
            SnapshotDate = inventorySnapshot.SnapshotDate,
            CreatedAt = inventorySnapshot.CreatedAt,
            CreatedBy = inventorySnapshot.CreatedBy
        };
    }
} 