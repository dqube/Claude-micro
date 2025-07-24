using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserDeactivatedEvent : DomainEventBase
{
    public UserId UserId { get; }

    public UserDeactivatedEvent(UserId userId)
    {
        UserId = userId;
    }
}