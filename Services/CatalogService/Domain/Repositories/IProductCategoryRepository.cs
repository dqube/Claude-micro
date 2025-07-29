using BuildingBlocks.Domain.Repository;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory, CategoryId>
{
    Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetByParentCategoryIdAsync(CategoryId parentCategoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetCategoryHierarchyAsync(CategoryId rootCategoryId, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasChildCategoriesAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
}