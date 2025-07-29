using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace CatalogService.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly CatalogDbContext _context;

    public ProductCategoryRepository(CatalogDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<ProductCategory?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> FindAsync(
        Expression<Func<ProductCategory, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.ProductCategories.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ProductCategory?> FindFirstAsync(
        Expression<Func<ProductCategory, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.ProductCategories.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<ProductCategory?> FindSingleAsync(
        Expression<Func<ProductCategory, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.ProductCategories.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> FindAsync(
        ISpecification<ProductCategory> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<ProductCategory?> FindFirstAsync(
        ISpecification<ProductCategory> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<ProductCategory, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.ProductCategories.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<ProductCategory> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.ProductCategories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<ProductCategory, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.ProductCategories.AnyAsync(predicate, cancellationToken);
    }

    public async Task<ProductCategory> AddAsync(ProductCategory entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.ProductCategories.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<ProductCategory>> AddRangeAsync(IEnumerable<ProductCategory> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.ProductCategories.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(ProductCategory entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.ProductCategories.Update(entity);
    }

    public void UpdateRange(IEnumerable<ProductCategory> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.ProductCategories.UpdateRange(entities);
    }

    public void Delete(ProductCategory entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.ProductCategories.Remove(entity);
    }

    public void DeleteRange(IEnumerable<ProductCategory> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.ProductCategories.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var category = await GetByIdAsync(id, cancellationToken);
        if (category is null) return false;
        
        Delete(category);
        return true;
    }

    // Domain-specific methods
    public async Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetByParentCategoryIdAsync(CategoryId parentCategoryId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parentCategoryId);
        return await _context.ProductCategories
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetCategoryHierarchyAsync(CategoryId rootCategoryId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rootCategoryId);
        var categories = new List<ProductCategory>();
        var queue = new Queue<CategoryId>();
        queue.Enqueue(rootCategoryId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var category = await GetByIdAsync(currentId, cancellationToken);
            
            if (category != null)
            {
                categories.Add(category);
                var children = await GetByParentCategoryIdAsync(currentId, cancellationToken);
                
                foreach (var child in children)
                {
                    queue.Enqueue(child.Id);
                }
            }
        }

        return categories;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.ProductCategories
            .AnyAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<bool> HasChildCategoriesAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(categoryId);
        return await _context.ProductCategories
            .AnyAsync(c => c.ParentCategoryId == categoryId, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(categoryId);
        return await _context.Products
            .AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    private IQueryable<ProductCategory> ApplySpecification(ISpecification<ProductCategory> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.ProductCategories.AsQueryable();

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