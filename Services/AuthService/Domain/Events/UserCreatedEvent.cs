using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserCreatedEvent : DomainEventBase
{
    public UserId UserId { get; }
    public Username Username { get; }
    public Email Email { get; }

    public UserCreatedEvent(UserId userId, Username username, Email email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}