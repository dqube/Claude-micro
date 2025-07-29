using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Events;

public sealed class CustomerEmailUpdatedEvent : DomainEventBase
{
    public CustomerId CustomerId { get; }
    public Email OldEmail { get; }
    public Email NewEmail { get; }

    public CustomerEmailUpdatedEvent(CustomerId customerId, Email oldEmail, Email newEmail)
    {
        CustomerId = customerId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
    }
} 