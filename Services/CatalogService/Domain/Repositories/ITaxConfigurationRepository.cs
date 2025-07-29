using BuildingBlocks.Domain.Repository;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Repositories;

public interface ITaxConfigurationRepository : IRepository<TaxConfiguration, TaxConfigId>
{
    Task<IEnumerable<TaxConfiguration>> GetByLocationIdAsync(int locationId, CancellationToken cancellationToken = default);
    Task<TaxConfiguration?> GetByLocationAndCategoryAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaxConfiguration>> GetApplicableConfigurationsAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default);
    Task<bool> ConfigurationExistsAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default);
}