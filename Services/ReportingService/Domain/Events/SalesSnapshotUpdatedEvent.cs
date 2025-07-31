using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class SalesSnapshotUpdatedEvent : DomainEventBase
{
    public SalesSnapshotId SalesSnapshotId { get; }
    public decimal OldTotalAmount { get; }
    public decimal NewTotalAmount { get; }

    public SalesSnapshotUpdatedEvent(
        SalesSnapshotId salesSnapshotId,
        decimal oldTotalAmount,
        decimal newTotalAmount)
    {
        SalesSnapshotId = salesSnapshotId;
        OldTotalAmount = oldTotalAmount;
        NewTotalAmount = newTotalAmount;
    }
} 