using BuildingBlocks.Application.CQRS.Commands;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Commands;

public class CreateStockMovementCommand : CommandBase<StockMovementDto>
{
    public Guid ProductId { get; init; }
    public int StoreId { get; init; }
    public int QuantityChange { get; init; }
    public string MovementType { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid? ReferenceId { get; init; }
    public Guid? CreatedBy { get; init; }

    public CreateStockMovementCommand(
        Guid productId,
        int storeId,
        int quantityChange,
        string movementType,
        Guid employeeId,
        Guid? referenceId = null,
        Guid? createdBy = null)
    {
        ProductId = productId;
        StoreId = storeId;
        QuantityChange = quantityChange;
        MovementType = movementType ?? throw new ArgumentNullException(nameof(movementType));
        EmployeeId = employeeId;
        ReferenceId = referenceId;
        CreatedBy = createdBy;
    }
}