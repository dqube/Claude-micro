namespace InventoryService.Application.DTOs;

public record StockMovementDto(
    Guid MovementId,
    Guid ProductId,
    int StoreId,
    int QuantityChange,
    string MovementType,
    DateTime MovementDate,
    Guid EmployeeId,
    Guid? ReferenceId,
    DateTime CreatedAt,
    Guid? CreatedBy);

public record CreateStockMovementDto(
    Guid ProductId,
    int StoreId,
    int QuantityChange,
    string MovementType,
    Guid EmployeeId,
    Guid? ReferenceId = null);