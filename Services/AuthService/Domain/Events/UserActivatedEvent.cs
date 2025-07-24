using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserActivatedEvent : DomainEventBase
{
    public UserId UserId { get; }

    public UserActivatedEvent(UserId userId)
    {
        UserId = userId;
    }
}