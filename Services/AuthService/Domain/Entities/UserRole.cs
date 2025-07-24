using BuildingBlocks.Domain.Entities;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class UserRole : AggregateRoot<UserId>
{
    public RoleId RoleId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private UserRole() : base(UserId.New())
    {
        RoleId = RoleId.From(0);
    }

    public UserRole(UserId userId, RoleId roleId, UserId? assignedBy = null) : base(userId)
    {
        RoleId = roleId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = assignedBy;

        AddDomainEvent(new UserRoleAssignedEvent(Id, RoleId));
    }
}