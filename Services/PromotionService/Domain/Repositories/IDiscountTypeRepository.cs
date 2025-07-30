using BuildingBlocks.Domain.Repository;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Repositories;

public interface IDiscountTypeRepository : IRepository<DiscountType, DiscountTypeId>, IReadOnlyRepository<DiscountType, DiscountTypeId>
{
    /// <summary>
    /// Gets a discount type by name
    /// </summary>
    /// <param name="name">The discount type name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The discount type if found, null otherwise</returns>
    Task<DiscountType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a discount type name already exists
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all discount types ordered by name
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of discount types</returns>
    Task<IEnumerable<DiscountType>> GetAllOrderedByNameAsync(CancellationToken cancellationToken = default);
} 