using BuildingBlocks.Domain.Repository;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Repositories;

public interface IPromotionRepository : IRepository<Promotion, PromotionId>, IReadOnlyRepository<Promotion, PromotionId>
{
    /// <summary>
    /// Gets a promotion by name
    /// </summary>
    /// <param name="name">The promotion name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The promotion if found, null otherwise</returns>
    Task<Promotion?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets currently active promotions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active promotions</returns>
    Task<IEnumerable<Promotion>> GetActivePromotionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets promotions active within a date range
    /// </summary>
    /// <param name="startDate">Start date range</param>
    /// <param name="endDate">End date range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of promotions active in the date range</returns>
    Task<IEnumerable<Promotion>> GetPromotionsInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets combinable promotions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of combinable promotions</returns>
    Task<IEnumerable<Promotion>> GetCombinablePromotionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets promotions ending soon (within specified days)
    /// </summary>
    /// <param name="days">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of promotions ending soon</returns>
    Task<IEnumerable<Promotion>> GetPromotionsEndingSoonAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a promotion name already exists
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets promotion with its products included
    /// </summary>
    /// <param name="promotionId">The promotion ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The promotion with products if found, null otherwise</returns>
    Task<Promotion?> GetWithProductsAsync(PromotionId promotionId, CancellationToken cancellationToken = default);
} 