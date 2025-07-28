using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class RegisterClosedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public StoreId StoreId { get; }
    public decimal EndingCash { get; }
    public decimal Variance { get; }

    public RegisterClosedEvent(RegisterId registerId, StoreId storeId, decimal endingCash, decimal variance)
    {
        RegisterId = registerId;
        StoreId = storeId;
        EndingCash = endingCash;
        Variance = variance;
    }
} 