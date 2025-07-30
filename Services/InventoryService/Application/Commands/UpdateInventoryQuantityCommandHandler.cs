using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Exceptions;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Commands;

public class UpdateInventoryQuantityCommandHandler : ICommandHandler<UpdateInventoryQuantityCommand, InventoryItemDto>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryQuantityCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryItemDto> HandleAsync(UpdateInventoryQuantityCommand request, CancellationToken cancellationToken = default)
    {
        var inventoryItemId = InventoryItemId.From(request.InventoryItemId);
        var updatedBy = EmployeeId.From(request.UpdatedBy);

        var inventoryItem = await _inventoryItemRepository
            .GetByIdAsync(inventoryItemId, cancellationToken);

        if (inventoryItem == null)
            throw new InventoryItemNotFoundException(inventoryItemId);

        inventoryItem.UpdateQuantity(request.NewQuantity, updatedBy);

        // Entity is already tracked by EF, no need to explicitly update
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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