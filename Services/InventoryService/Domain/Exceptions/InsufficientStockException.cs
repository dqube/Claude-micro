using BuildingBlocks.Domain.Exceptions;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public ProductId ProductId { get; }
    public StoreId StoreId { get; }
    public int RequiredQuantity { get; }
    public int AvailableQuantity { get; }

    public InsufficientStockException(
        ProductId productId,
        StoreId storeId,
        int requiredQuantity,
        int availableQuantity)
        : base($"Insufficient stock for product '{productId}' in store '{storeId}'. Required: {requiredQuantity}, Available: {availableQuantity}")
    {
        ProductId = productId;
        StoreId = storeId;
        RequiredQuantity = requiredQuantity;
        AvailableQuantity = availableQuantity;
    }
}