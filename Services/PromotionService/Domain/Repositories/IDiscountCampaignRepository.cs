using BuildingBlocks.Domain.Repository;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Repositories;

public interface IDiscountCampaignRepository : IRepository<DiscountCampaign, CampaignId>, IReadOnlyRepository<DiscountCampaign, CampaignId>
{
    /// <summary>
    /// Gets a campaign by name
    /// </summary>
    /// <param name="name">The campaign name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The campaign if found, null otherwise</returns>
    Task<DiscountCampaign?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active campaigns
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active campaigns</returns>
    Task<IEnumerable<DiscountCampaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets campaigns active within a date range
    /// </summary>
    /// <param name="startDate">Start date range</param>
    /// <param name="endDate">End date range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of campaigns active in the date range</returns>
    Task<IEnumerable<DiscountCampaign>> GetCampaignsInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets campaigns ending soon (within specified days)
    /// </summary>
    /// <param name="days">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of campaigns ending soon</returns>
    Task<IEnumerable<DiscountCampaign>> GetCampaignsEndingSoonAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a campaign name already exists
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets campaigns with their rules included
    /// </summary>
    /// <param name="campaignId">The campaign ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The campaign with rules if found, null otherwise</returns>
    Task<DiscountCampaign?> GetWithRulesAsync(CampaignId campaignId, CancellationToken cancellationToken = default);
} 