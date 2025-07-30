using BuildingBlocks.Domain.Repository;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Repositories;

public interface IDiscountRuleRepository : IRepository<DiscountRule, RuleId>, IReadOnlyRepository<DiscountRule, RuleId>
{
    Task<IEnumerable<DiscountRule>> GetByCampaignIdAsync(CampaignId campaignId, CancellationToken cancellationToken = default);
} 