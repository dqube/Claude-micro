using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.Events;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Entities;

public class InventoryItem : AggregateRoot<InventoryItemId>
{
    public StoreId StoreId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int ReorderLevel { get; private set; }
    public DateTime? LastRestockDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public EmployeeId? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public EmployeeId? UpdatedBy { get; private set; }

    private InventoryItem() : base(InventoryItemId.New())
    {
        StoreId = StoreId.From(1);
        ProductId = ProductId.New();
    }

    public InventoryItem(
        InventoryItemId id,
        StoreId storeId,
        ProductId productId,
        int quantity = 0,
        int reorderLevel = 10,
        EmployeeId? createdBy = null) : base(id)
    {
        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        Quantity = Math.Max(0, quantity);
        ReorderLevel = Math.Max(0, reorderLevel);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        AddDomainEvent(new InventoryItemCreatedEvent(Id, StoreId, ProductId, Quantity));
    }

    public void UpdateQuantity(int newQuantity, EmployeeId updatedBy)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(newQuantity));

        var oldQuantity = Quantity;
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new InventoryQuantityUpdatedEvent(Id, StoreId, ProductId, oldQuantity, newQuantity));

        if (IsLowStock())
        {
            AddDomainEvent(new LowStockDetectedEvent(Id, StoreId, ProductId, Quantity, ReorderLevel));
        }
    }

    public void AdjustQuantity(int quantityChange, EmployeeId adjustedBy)
    {
        var newQuantity = Quantity + quantityChange;
        if (newQuantity < 0)
            throw new ArgumentException("Resulting quantity cannot be negative", nameof(quantityChange));

        UpdateQuantity(newQuantity, adjustedBy);
    }

    public void UpdateReorderLevel(int newReorderLevel, EmployeeId updatedBy)
    {
        if (newReorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(newReorderLevel));

        ReorderLevel = newReorderLevel;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        if (IsLowStock())
        {
            AddDomainEvent(new LowStockDetectedEvent(Id, StoreId, ProductId, Quantity, ReorderLevel));
        }
    }

    public void RecordRestock(DateTime restockDate, EmployeeId restockedBy)
    {
        LastRestockDate = restockDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = restockedBy;

        AddDomainEvent(new InventoryRestockedEvent(Id, StoreId, ProductId, restockDate));
    }

    public bool IsLowStock() => Quantity <= ReorderLevel;

    public bool IsOutOfStock() => Quantity == 0;
}