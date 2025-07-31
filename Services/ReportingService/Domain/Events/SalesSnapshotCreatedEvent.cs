using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class SalesSnapshotCreatedEvent : DomainEventBase
{
    public SalesSnapshotId SalesSnapshotId { get; }
    public SaleId SaleId { get; }
    public StoreId StoreId { get; }
    public DateOnly SaleDate { get; }
    public decimal TotalAmount { get; }

    public SalesSnapshotCreatedEvent(
        SalesSnapshotId salesSnapshotId,
        SaleId saleId,
        StoreId storeId,
        DateOnly saleDate,
        decimal totalAmount)
    {
        SalesSnapshotId = salesSnapshotId;
        SaleId = saleId;
        StoreId = storeId;
        SaleDate = saleDate;
        TotalAmount = totalAmount;
    }
} 