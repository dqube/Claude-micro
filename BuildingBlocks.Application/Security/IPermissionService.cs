namespace BuildingBlocks.Application.Security;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task<bool> HasAnyPermissionAsync(IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task<bool> HasAnyPermissionAsync(string userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task<bool> HasAllPermissionsAsync(IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task<bool> HasAllPermissionsAsync(string userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    Task<bool> IsInAnyRoleAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> IsInAnyRoleAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}