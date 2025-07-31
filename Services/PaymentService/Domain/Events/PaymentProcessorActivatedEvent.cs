using BuildingBlocks.Domain.DomainEvents;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Events;

public class PaymentProcessorActivatedEvent : DomainEventBase
{
    public PaymentProcessorId ProcessorId { get; }
    public string Name { get; }

    public PaymentProcessorActivatedEvent(PaymentProcessorId processorId, string name)
    {
        ProcessorId = processorId;
        Name = name;
    }
} 