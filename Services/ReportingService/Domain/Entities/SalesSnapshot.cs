using BuildingBlocks.Domain.Entities;
using ReportingService.Domain.Events;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Entities;

public class SalesSnapshot : AggregateRoot<SalesSnapshotId>
{
    public SaleId SaleId { get; private set; }
    public StoreId StoreId { get; private set; }
    public DateOnly SaleDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public CustomerId? CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private SalesSnapshot() : base(SalesSnapshotId.New())
    {
        SaleId = SaleId.New();
        StoreId = StoreId.From(1);
        SaleDate = DateOnly.FromDateTime(DateTime.UtcNow);
        TotalAmount = 0;
    }

    public SalesSnapshot(
        SalesSnapshotId id,
        SaleId saleId,
        StoreId storeId,
        DateOnly saleDate,
        decimal totalAmount,
        CustomerId? customerId = null) : base(id)
    {
        SaleId = saleId ?? throw new ArgumentNullException(nameof(saleId));
        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        SaleDate = saleDate;
        TotalAmount = totalAmount >= 0 ? totalAmount : throw new ArgumentException("Total amount cannot be negative", nameof(totalAmount));
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new SalesSnapshotCreatedEvent(Id, SaleId, StoreId, SaleDate, TotalAmount));
    }

    public void UpdateTotalAmount(decimal newTotalAmount)
    {
        if (newTotalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative", nameof(newTotalAmount));

        var oldAmount = TotalAmount;
        TotalAmount = newTotalAmount;

        AddDomainEvent(new SalesSnapshotUpdatedEvent(Id, oldAmount, newTotalAmount));
    }
} 