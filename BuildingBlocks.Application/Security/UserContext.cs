namespace BuildingBlocks.Application.Security;

public class UserContext
{
    public UserContext()
    {
        Permissions = new List<string>();
        Roles = new List<string>();
        Claims = new Dictionary<string, string>();
    }

    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsAuthenticated { get; set; }
    public IList<string> Permissions { get; set; }
    public IList<string> Roles { get; set; }
    public Dictionary<string, string> Claims { get; set; }
    public string? TenantId { get; set; }
    public string? OrganizationId { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }

    public bool HasAnyPermission(params string[] permissions)
    {
        return permissions.Any(HasPermission);
    }

    public bool HasAllPermissions(params string[] permissions)
    {
        return permissions.All(HasPermission);
    }

    public bool IsInRole(string role)
    {
        return Roles.Contains(role);
    }

    public bool IsInAnyRole(params string[] roles)
    {
        return roles.Any(IsInRole);
    }

    public bool IsInAllRoles(params string[] roles)
    {
        return roles.All(IsInRole);
    }

    public string? GetClaim(string claimType)
    {
        return Claims.TryGetValue(claimType, out var value) ? value : null;
    }

    public void AddClaim(string claimType, string claimValue)
    {
        Claims[claimType] = claimValue;
    }

    public void AddPermission(string permission)
    {
        if (!Permissions.Contains(permission))
        {
            Permissions.Add(permission);
        }
    }

    public void AddRole(string role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
        }
    }
}