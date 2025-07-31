using BuildingBlocks.Domain.DomainEvents;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Events;

public class PaymentProcessorUpdatedEvent : DomainEventBase
{
    public PaymentProcessorId ProcessorId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public decimal OldCommissionRate { get; }
    public decimal NewCommissionRate { get; }

    public PaymentProcessorUpdatedEvent(
        PaymentProcessorId processorId,
        string oldName,
        string newName,
        decimal oldCommissionRate,
        decimal newCommissionRate)
    {
        ProcessorId = processorId;
        OldName = oldName;
        NewName = newName;
        OldCommissionRate = oldCommissionRate;
        NewCommissionRate = newCommissionRate;
    }
} 