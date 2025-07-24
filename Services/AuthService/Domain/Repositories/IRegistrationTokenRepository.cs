using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace AuthService.Domain.Repositories;

public interface IRegistrationTokenRepository : IRepository<RegistrationToken, TokenId>, IReadOnlyRepository<RegistrationToken, TokenId>
{
    /// <summary>
    /// Gets a registration token by its token string value
    /// </summary>
    /// <param name="token">The token string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The registration token if found, null otherwise</returns>
    Task<RegistrationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a valid (unused and not expired) registration token by its token string value
    /// </summary>
    /// <param name="token">The token string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The valid registration token if found, null otherwise</returns>
    Task<RegistrationToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registration tokens for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of registration tokens for the user</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registration tokens for a specific email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of registration tokens for the email</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registration tokens of a specific type
    /// </summary>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of registration tokens of the specified type</returns>
    Task<IEnumerable<RegistrationToken>> GetTokensByTypeAsync(TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expired registration tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of expired registration tokens</returns>
    Task<IEnumerable<RegistrationToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unused registration tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of unused registration tokens</returns>
    Task<IEnumerable<RegistrationToken>> GetUnusedTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest valid token for a user of a specific type
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="tokenType">The token type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The latest valid token if found, null otherwise</returns>
    Task<RegistrationToken?> GetLatestValidTokenAsync(UserId userId, TokenType tokenType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired tokens by removing them from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of tokens that were cleaned up</returns>
    Task<int> CleanExpiredTokensAsync(CancellationToken cancellationToken = default);
}