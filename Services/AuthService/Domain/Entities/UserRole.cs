using BuildingBlocks.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class UserRole : AuditableEntity
{
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

    public User User { get; private set; }
    public Role Role { get; private set; }

    // Private constructor for EF Core
    private UserRole() { }

    public UserRole(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}