namespace InventoryService.Application.DTOs;

public record InventoryItemDto(
    Guid InventoryItemId,
    int StoreId,
    Guid ProductId,
    int Quantity,
    int ReorderLevel,
    DateTime? LastRestockDate,
    DateTime CreatedAt,
    Guid? CreatedBy,
    DateTime? UpdatedAt,
    Guid? UpdatedBy);

public record CreateInventoryItemDto(
    int StoreId,
    Guid ProductId,
    int Quantity = 0,
    int ReorderLevel = 10);

public record UpdateInventoryQuantityDto(
    Guid InventoryItemId,
    int NewQuantity);

public record AdjustInventoryQuantityDto(
    Guid InventoryItemId,
    int QuantityChange);

public record UpdateReorderLevelDto(
    Guid InventoryItemId,
    int NewReorderLevel);