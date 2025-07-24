using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserPasswordUpdatedEvent : DomainEventBase
{
    public UserId UserId { get; }

    public UserPasswordUpdatedEvent(UserId userId)
    {
        UserId = userId;
    }
}