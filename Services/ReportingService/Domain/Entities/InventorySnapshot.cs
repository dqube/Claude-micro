using BuildingBlocks.Domain.Entities;
using ReportingService.Domain.Events;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Entities;

public class InventorySnapshot : AggregateRoot<InventorySnapshotId>
{
    public ProductId ProductId { get; private set; }
    public StoreId StoreId { get; private set; }
    public int Quantity { get; private set; }
    public DateOnly SnapshotDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private InventorySnapshot() : base(InventorySnapshotId.New())
    {
        ProductId = ProductId.New();
        StoreId = StoreId.From(1);
        Quantity = 0;
        SnapshotDate = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public InventorySnapshot(
        InventorySnapshotId id,
        ProductId productId,
        StoreId storeId,
        int quantity,
        DateOnly? snapshotDate = null) : base(id)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        Quantity = quantity >= 0 ? quantity : throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        SnapshotDate = snapshotDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new InventorySnapshotCreatedEvent(Id, ProductId, StoreId, Quantity, SnapshotDate));
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(newQuantity));

        var oldQuantity = Quantity;
        Quantity = newQuantity;

        AddDomainEvent(new InventorySnapshotUpdatedEvent(Id, oldQuantity, newQuantity));
    }
} 