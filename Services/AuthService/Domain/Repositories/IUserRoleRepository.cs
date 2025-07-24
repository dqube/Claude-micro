using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Repositories;

public interface IUserRoleRepository : IRepository<UserRole, UserId>, IReadOnlyRepository<UserRole, UserId>
{
    /// <summary>
    /// Gets all user roles for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of user roles for the user</returns>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user roles for a specific role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of user roles for the role</returns>
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user role by user ID and role ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user role if found, null otherwise</returns>
    Task<UserRole?> GetByUserIdAndRoleIdAsync(UserId userId, RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of roles assigned to the user</returns>
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users for a specific role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of users assigned to the role</returns>
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(UserId userId, RoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all roles from a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of roles removed</returns>
    Task<int> RemoveAllRolesFromUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all users from a role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of users removed from the role</returns>
    Task<int> RemoveAllUsersFromRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
}