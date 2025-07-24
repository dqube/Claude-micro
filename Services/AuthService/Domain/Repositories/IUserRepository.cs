using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace AuthService.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>, IReadOnlyRepository<User, UserId>
{
    /// <summary>
    /// Gets a user by their username
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active users
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all inactive users
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inactive users</returns>
    Task<IEnumerable<User>> GetInactiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all locked users
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of locked users</returns>
    Task<IEnumerable<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by username or email
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of users matching the search term</returns>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username already exists
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if username exists, false otherwise</returns>
    Task<bool> UsernameExistsAsync(Username username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email already exists
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
}