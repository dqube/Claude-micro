using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class UserRoleAssignedEvent : DomainEventBase
{
    public UserId UserId { get; }
    public RoleId RoleId { get; }
    public UserId? AssignedBy { get; }

    public UserRoleAssignedEvent(UserId userId, RoleId roleId, UserId? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
    }
}