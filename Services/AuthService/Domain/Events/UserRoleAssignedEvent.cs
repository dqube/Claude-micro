using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserRoleAssignedEvent : DomainEventBase
{
    public UserId UserId { get; }
    public RoleId RoleId { get; }

    public UserRoleAssignedEvent(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}