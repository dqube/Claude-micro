using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Commands;

public class CreateInventorySnapshotCommandHandler : ICommandHandler<CreateInventorySnapshotCommand, InventorySnapshotDto>
{
    private readonly IInventorySnapshotRepository _inventorySnapshotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventorySnapshotCommandHandler(
        IInventorySnapshotRepository inventorySnapshotRepository,
        IUnitOfWork unitOfWork)
    {
        _inventorySnapshotRepository = inventorySnapshotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventorySnapshotDto> HandleAsync(CreateInventorySnapshotCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create value objects
        var productId = ProductId.From(request.ProductId);
        var storeId = StoreId.From(request.StoreId);

        // Create entity
        var inventorySnapshot = new InventorySnapshot(
            InventorySnapshotId.New(),
            productId,
            storeId,
            request.Quantity,
            request.SnapshotDate);

        // Add to repository
        await _inventorySnapshotRepository.AddAsync(inventorySnapshot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(inventorySnapshot);
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