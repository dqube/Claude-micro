namespace BuildingBlocks.Application.Security;

public class SecurityContext
{
    private readonly AsyncLocal<UserContext?> _userContext = new();

    public UserContext? CurrentUser
    {
        get => _userContext.Value;
        set => _userContext.Value = value;
    }

    public bool IsAuthenticated => CurrentUser?.IsAuthenticated ?? false;

    public string? UserId => CurrentUser?.UserId;

    public string? UserName => CurrentUser?.UserName;

    public string? Email => CurrentUser?.Email;

    public string? TenantId => CurrentUser?.TenantId;

    public string? OrganizationId => CurrentUser?.OrganizationId;

    public void SetUser(UserContext userContext)
    {
        CurrentUser = userContext;
    }

    public void SetUser(string userId, string userName, string email, bool isAuthenticated = true)
    {
        CurrentUser = new UserContext
        {
            UserId = userId,
            UserName = userName,
            Email = email,
            IsAuthenticated = isAuthenticated
        };
    }

    public void Clear()
    {
        CurrentUser = null;
    }

    public bool HasPermission(string permission)
    {
        return CurrentUser?.HasPermission(permission) ?? false;
    }

    public bool HasAnyPermission(params string[] permissions)
    {
        return CurrentUser?.HasAnyPermission(permissions) ?? false;
    }

    public bool HasAllPermissions(params string[] permissions)
    {
        return CurrentUser?.HasAllPermissions(permissions) ?? false;
    }

    public bool IsInRole(string role)
    {
        return CurrentUser?.IsInRole(role) ?? false;
    }

    public bool IsInAnyRole(params string[] roles)
    {
        return CurrentUser?.IsInAnyRole(roles) ?? false;
    }

    public bool IsInAllRoles(params string[] roles)
    {
        return CurrentUser?.IsInAllRoles(roles) ?? false;
    }

    public string? GetClaim(string claimType)
    {
        return CurrentUser?.GetClaim(claimType);
    }
}