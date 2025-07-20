namespace BuildingBlocks.Infrastructure.Authorization;

public interface IAuthorizationService
{
    Task<bool> AuthorizeAsync(string userId, string resource, string action);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string roleName);
}