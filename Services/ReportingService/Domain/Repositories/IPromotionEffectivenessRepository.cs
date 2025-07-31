using BuildingBlocks.Domain.Repository;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace ReportingService.Domain.Repositories;

public interface IPromotionEffectivenessRepository : IRepository<PromotionEffectiveness, PromotionEffectivenessId>, IReadOnlyRepository<PromotionEffectiveness, PromotionEffectivenessId>
{
    /// <summary>
    /// Gets promotion effectiveness by promotion ID
    /// </summary>
    /// <param name="promotionId">The promotion ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Promotion effectiveness for the promotion</returns>
    Task<PromotionEffectiveness?> GetByPromotionIdAsync(PromotionId promotionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets promotion effectiveness records within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of promotion effectiveness records within the date range</returns>
    Task<IEnumerable<PromotionEffectiveness>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top performing promotions by redemption count
    /// </summary>
    /// <param name="topCount">Number of top promotions to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of top performing promotions by redemption count</returns>
    Task<IEnumerable<PromotionEffectiveness>> GetTopByRedemptionCountAsync(int topCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top performing promotions by revenue impact
    /// </summary>
    /// <param name="topCount">Number of top promotions to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of top performing promotions by revenue impact</returns>
    Task<IEnumerable<PromotionEffectiveness>> GetTopByRevenueImpactAsync(int topCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets promotions with low effectiveness (below threshold)
    /// </summary>
    /// <param name="redemptionThreshold">Minimum redemption count threshold</param>
    /// <param name="revenueThreshold">Minimum revenue impact threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of underperforming promotions</returns>
    Task<IEnumerable<PromotionEffectiveness>> GetUnderperformingPromotionsAsync(int redemptionThreshold, decimal revenueThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets average redemption count for promotions within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Average redemption count</returns>
    Task<double> GetAverageRedemptionCountAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total revenue impact for all promotions within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total revenue impact</returns>
    Task<decimal> GetTotalRevenueImpactAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
} 