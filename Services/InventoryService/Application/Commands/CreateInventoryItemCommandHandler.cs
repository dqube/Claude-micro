using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Commands;

public class CreateInventoryItemCommandHandler : ICommandHandler<CreateInventoryItemCommand, InventoryItemDto>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryItemDto> HandleAsync(CreateInventoryItemCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var storeId = StoreId.From(request.StoreId);
        var productId = ProductId.From(request.ProductId);

        var existingItem = await _inventoryItemRepository
            .GetByStoreAndProductAsync(storeId, productId, cancellationToken);

        if (existingItem != null)
            throw new InvalidOperationException($"Inventory item already exists for product {request.ProductId} in store {request.StoreId}");

        var createdBy = request.CreatedBy.HasValue ? EmployeeId.From(request.CreatedBy.Value) : null;

        var inventoryItem = new InventoryItem(
            InventoryItemId.New(),
            storeId,
            productId,
            request.Quantity,
            request.ReorderLevel,
            createdBy);

        await _inventoryItemRepository.AddAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(inventoryItem);
    }

    private static InventoryItemDto MapToDto(InventoryItem inventoryItem)
    {
        return new InventoryItemDto(
            inventoryItem.Id.Value,
            inventoryItem.StoreId.Value,
            inventoryItem.ProductId.Value,
            inventoryItem.Quantity,
            inventoryItem.ReorderLevel,
            inventoryItem.LastRestockDate,
            inventoryItem.CreatedAt,
            inventoryItem.CreatedBy?.Value,
            inventoryItem.UpdatedAt,
            inventoryItem.UpdatedBy?.Value);
    }
}