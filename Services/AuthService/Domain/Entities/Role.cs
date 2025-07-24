using BuildingBlocks.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class Role : Entity<RoleId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public UserId? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private Role() : base(RoleId.From(0))
    {
        Name = string.Empty;
    }

    public Role(RoleId id, string name, string? description = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));
        
        if (name.Length > 20)
            throw new ArgumentException("Role name cannot exceed 20 characters", nameof(name));

        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description, UserId updatedBy)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}