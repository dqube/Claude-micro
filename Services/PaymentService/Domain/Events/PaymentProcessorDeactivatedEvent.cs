using BuildingBlocks.Domain.DomainEvents;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Events;

public class PaymentProcessorDeactivatedEvent : DomainEventBase
{
    public PaymentProcessorId ProcessorId { get; }
    public string Name { get; }

    public PaymentProcessorDeactivatedEvent(PaymentProcessorId processorId, string name)
    {
        ProcessorId = processorId;
        Name = name;
    }
} 