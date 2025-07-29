using BuildingBlocks.Domain.Repository;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Repositories;

public interface IProductRepository : IRepository<Product, ProductId>
{
    Task<Product?> GetBySKUAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(BarcodeValue barcodeValue, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> SKUExistsAsync(SKU sku, CancellationToken cancellationToken = default);
    Task<bool> BarcodeExistsAsync(BarcodeValue barcodeValue, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetTaxableProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
}