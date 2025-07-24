using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId>, IReadOnlyRepository<Role, RoleId>
{
    /// <summary>
    /// Gets a role by its name
    /// </summary>
    /// <param name="name">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles ordered by name
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of roles ordered by name</returns>
    Task<IEnumerable<Role>> GetAllOrderedByNameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name already exists
    /// </summary>
    /// <param name="name">The role name to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if role name exists, false otherwise</returns>
    Task<bool> RoleNameExistsAsync(string name, CancellationToken cancellationToken = default);
}