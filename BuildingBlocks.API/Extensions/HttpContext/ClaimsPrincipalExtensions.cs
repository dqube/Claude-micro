using System.Security.Claims;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Extensions.HttpContext;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ApiConstants.ClaimTypes.UserId)?.Value ??
               principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               principal.FindFirst("sub")?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ??
               principal.FindFirst("name")?.Value;
    }

    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ApiConstants.ClaimTypes.Email)?.Value ??
               principal.FindFirst(ClaimTypes.Email)?.Value ??
               principal.FindFirst("email")?.Value;
    }

    public static string? GetUserRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ApiConstants.ClaimTypes.Role)?.Value ??
               principal.FindFirst(ClaimTypes.Role)?.Value ??
               principal.FindFirst("role")?.Value;
    }

    public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ApiConstants.ClaimTypes.Role)
            .Concat(principal.FindAll(ClaimTypes.Role))
            .Concat(principal.FindAll("role"))
            .Select(c => c.Value)
            .Distinct();
    }

    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ApiConstants.ClaimTypes.TenantId)?.Value ??
               principal.FindFirst("tenant_id")?.Value ??
               principal.FindFirst("tenant")?.Value;
    }

    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ApiConstants.ClaimTypes.Permission)
            .Concat(principal.FindAll("permission"))
            .Concat(principal.FindAll("permissions"))
            .Select(c => c.Value)
            .Distinct();
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        return principal.GetPermissions().Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public static bool HasAnyPermission(this ClaimsPrincipal principal, params string[] permissions)
    {
        var userPermissions = principal.GetPermissions();
        return permissions.Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));
    }

    public static bool HasAllPermissions(this ClaimsPrincipal principal, params string[] permissions)
    {
        var userPermissions = principal.GetPermissions();
        return permissions.All(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));
    }

    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal.GetUserRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        var userRoles = principal.GetUserRoles();
        return roles.Any(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public static bool IsInAllRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        var userRoles = principal.GetUserRoles();
        return roles.All(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirst(claimType)?.Value;
    }

    public static IEnumerable<string> GetClaimValues(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindAll(claimType).Select(c => c.Value);
    }

    public static bool HasClaim(this ClaimsPrincipal principal, string claimType)
    {
        return principal.HasClaim(c => c.Type == claimType);
    }

    public static bool HasClaim(this ClaimsPrincipal principal, string claimType, string claimValue)
    {
        return principal.HasClaim(claimType, claimValue);
    }

    public static Dictionary<string, object> GetUserInfo(this ClaimsPrincipal principal)
    {
        var userInfo = new Dictionary<string, object>();

        var userId = principal.GetUserId();
        if (!string.IsNullOrEmpty(userId))
            userInfo["userId"] = userId;

        var userName = principal.GetUserName();
        if (!string.IsNullOrEmpty(userName))
            userInfo["userName"] = userName;

        var email = principal.GetUserEmail();
        if (!string.IsNullOrEmpty(email))
            userInfo["email"] = email;

        var roles = principal.GetUserRoles().ToArray();
        if (roles.Length > 0)
            userInfo["roles"] = roles;

        var permissions = principal.GetPermissions().ToArray();
        if (permissions.Length > 0)
            userInfo["permissions"] = permissions;

        var tenantId = principal.GetTenantId();
        if (!string.IsNullOrEmpty(tenantId))
            userInfo["tenantId"] = tenantId;

        return userInfo;
    }

    public static bool IsSystemUser(this ClaimsPrincipal principal)
    {
        return principal.HasClaim("user_type", "system") ||
               principal.HasClaim("client_id", "system");
    }

    public static bool IsServiceAccount(this ClaimsPrincipal principal)
    {
        return principal.HasClaim("user_type", "service") ||
               principal.HasClaim("client_credentials", "true");
    }

    public static bool IsApiKeyUser(this ClaimsPrincipal principal)
    {
        return principal.Identity?.AuthenticationType == "ApiKey";
    }

    public static bool IsJwtUser(this ClaimsPrincipal principal)
    {
        return principal.Identity?.AuthenticationType == "Bearer" ||
               principal.Identity?.AuthenticationType == "JWT";
    }

    public static TimeSpan? GetTokenExpiry(this ClaimsPrincipal principal)
    {
        var expClaim = principal.FindFirst("exp")?.Value;
        if (long.TryParse(expClaim, out var exp))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(exp);
            return expiry - DateTimeOffset.UtcNow;
        }
        return null;
    }

    public static bool IsTokenExpired(this ClaimsPrincipal principal)
    {
        var expiry = principal.GetTokenExpiry();
        return expiry.HasValue && expiry.Value <= TimeSpan.Zero;
    }
}