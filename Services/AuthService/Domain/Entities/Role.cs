using BuildingBlocks.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class Role : Entity<RoleId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Private constructor for EF Core
    private Role() { }

    public Role(RoleId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}