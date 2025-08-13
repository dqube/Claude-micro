using BuildingBlocks.Domain.DomainEvents;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Events;

public class PaymentProcessorCreatedEvent : DomainEventBase
{
    public PaymentProcessorId ProcessorId { get; }
    public string Name { get; }
    public decimal CommissionRate { get; }
    public bool IsActive { get; }

    public PaymentProcessorCreatedEvent(
        PaymentProcessorId processorId,
        string name,
        decimal commissionRate,
        bool isActive)
    {
        ProcessorId = processorId;
        Name = name;
        CommissionRate = commissionRate;
        IsActive = isActive;
    }
} 