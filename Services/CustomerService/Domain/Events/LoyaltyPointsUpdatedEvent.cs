using BuildingBlocks.Domain.DomainEvents;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Events;

public sealed class LoyaltyPointsUpdatedEvent : DomainEventBase
{
    public CustomerId CustomerId { get; }
    public int PreviousPoints { get; }
    public int NewPoints { get; }
    public int PointsChange { get; }
    public string Reason { get; }

    public LoyaltyPointsUpdatedEvent(
        CustomerId customerId,
        int previousPoints,
        int newPoints,
        string reason)
    {
        CustomerId = customerId;
        PreviousPoints = previousPoints;
        NewPoints = newPoints;
        PointsChange = newPoints - previousPoints;
        Reason = reason;
    }
} 