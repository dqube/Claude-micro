using BuildingBlocks.Domain.Repository;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Repositories;

public interface IPromotionProductRepository : IRepository<PromotionProduct, PromotionProductId>, IReadOnlyRepository<PromotionProduct, PromotionProductId>
{
    Task<IEnumerable<PromotionProduct>> GetByPromotionIdAsync(PromotionId promotionId, CancellationToken cancellationToken = default);
} 