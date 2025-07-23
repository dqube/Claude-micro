using System.Globalization;
using System.Security.Claims;

namespace BuildingBlocks.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
               ?? principal?.FindFirst("sub")?.Value
               ?? principal?.FindFirst("user_id")?.Value
               ?? principal?.Identity?.Name;
    }

    public static Guid? GetUserIdAsGuid(this ClaimsPrincipal principal)
    {
        var userId = GetUserId(principal);
        return Guid.TryParse(userId, out var id) ? id : null;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.Name)?.Value
               ?? principal?.FindFirst("name")?.Value
               ?? principal?.FindFirst("username")?.Value
               ?? principal?.Identity?.Name;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.Email)?.Value
               ?? principal?.FindFirst("email")?.Value;
    }

    public static string? GetFirstName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.GivenName)?.Value
               ?? principal?.FindFirst("given_name")?.Value
               ?? principal?.FindFirst("first_name")?.Value;
    }

    public static string? GetLastName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.Surname)?.Value
               ?? principal?.FindFirst("family_name")?.Value
               ?? principal?.FindFirst("last_name")?.Value;
    }

    public static string GetFullName(this ClaimsPrincipal principal)
    {
        var firstName = GetFirstName(principal);
        var lastName = GetLastName(principal);

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
        {
            return $"{firstName} {lastName}";
        }

        return GetUserName(principal) ?? GetEmail(principal) ?? "Unknown User";
    }

    public static string[] GetRoles(this ClaimsPrincipal principal)
    {
        return principal?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray() ?? Array.Empty<string>();
    }

    public static bool HasRole(this ClaimsPrincipal principal, string role)
    {
        return principal?.IsInRole(role) == true;
    }

    public static bool HasAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.Any(role => principal?.IsInRole(role) == true);
    }

    public static bool HasAllRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.All(role => principal?.IsInRole(role) == true);
    }

    public static string? GetClaim(this ClaimsPrincipal principal, string claimType)
    {
        return principal?.FindFirst(claimType)?.Value;
    }

    public static T? GetClaim<T>(this ClaimsPrincipal principal, string claimType)
    {
        var claimValue = GetClaim(principal, claimType);
        if (string.IsNullOrEmpty(claimValue))
            return default;

        try
        {
            return (T)Convert.ChangeType(claimValue, typeof(T), CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            return default;
        }
        catch (FormatException)
        {
            return default;
        }
        catch (OverflowException)
        {
            return default;
        }
    }

    public static string[] GetClaimValues(this ClaimsPrincipal principal, string claimType)
    {
        return principal?.FindAll(claimType)
            .Select(c => c.Value)
            .ToArray() ?? Array.Empty<string>();
    }

    public static bool HasClaim(this ClaimsPrincipal principal, string claimType)
    {
        return principal?.HasClaim(claimType, string.Empty) == true || principal?.FindFirst(claimType) != null;
    }

    public static bool HasClaim(this ClaimsPrincipal principal, string claimType, string claimValue)
    {
        return principal?.HasClaim(claimType, claimValue) == true;
    }

    public static string? GetTenant(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst("tenant")?.Value
               ?? principal?.FindFirst("tenant_id")?.Value
               ?? principal?.FindFirst("organization")?.Value
               ?? principal?.FindFirst("org_id")?.Value;
    }

    public static string[] GetPermissions(this ClaimsPrincipal principal)
    {
        return principal?.FindAll("permission")
            .Select(c => c.Value)
            .ToArray() ?? Array.Empty<string>();
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        var permissions = GetPermissions(principal);
        return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public static bool HasAnyPermission(this ClaimsPrincipal principal, params string[] permissions)
    {
        var userPermissions = GetPermissions(principal);
        return permissions.Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));
    }

    public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        return HasAnyRole(principal, roles);
    }

    public static IDictionary<string, string> GetAllClaims(this ClaimsPrincipal principal)
    {
        return principal?.Claims.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
    }

    public static bool IsAnonymous(this ClaimsPrincipal principal)
    {
        return principal?.Identity?.IsAuthenticated != true;
    }

    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        return principal?.Identity?.IsAuthenticated == true;
    }

    public static string GetAuthenticationType(this ClaimsPrincipal principal)
    {
        return principal?.Identity?.AuthenticationType ?? "Unknown";
    }
}