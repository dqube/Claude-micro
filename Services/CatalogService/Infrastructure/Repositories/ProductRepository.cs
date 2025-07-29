using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace CatalogService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> FindAsync(
        Expression<Func<Product, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Product?> FindFirstAsync(
        Expression<Func<Product, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Product?> FindSingleAsync(
        Expression<Func<Product, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> FindAsync(
        ISpecification<Product> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Product?> FindFirstAsync(
        ISpecification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Product, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Products.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Product, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Products.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Products.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Products.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Products.Update(entity);
    }

    public void UpdateRange(IEnumerable<Product> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Products.UpdateRange(entities);
    }

    public void Delete(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Products.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Product> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Products.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var product = await GetByIdAsync(id, cancellationToken);
        if (product is null) return false;
        
        Delete(product);
        return true;
    }

    // Domain-specific methods
    public async Task<Product?> GetBySKUAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sku);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<Product?> GetByBarcodeAsync(BarcodeValue barcodeValue, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(barcodeValue);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .FirstOrDefaultAsync(p => p.Barcodes.Any(b => b.BarcodeValue == barcodeValue), cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(categoryId);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .Where(p => p.Name.Contains(searchTerm) || 
                       (p.Description != null && p.Description.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SKUExistsAsync(SKU sku, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sku);
        return await _context.Products
            .AnyAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<bool> BarcodeExistsAsync(BarcodeValue barcodeValue, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(barcodeValue);
        return await _context.ProductBarcodes
            .AnyAsync(b => b.BarcodeValue == barcodeValue, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetTaxableProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .Where(p => p.IsTaxable)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .Where(p => p.BasePrice.Value >= minPrice && p.BasePrice.Value <= maxPrice)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Product> ApplySpecification(ISpecification<Product> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Products
            .Include(p => p.Barcodes)
            .Include(p => p.CountryPricing)
            .AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }

        if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(g => g);
        }

        return query;
    }
}