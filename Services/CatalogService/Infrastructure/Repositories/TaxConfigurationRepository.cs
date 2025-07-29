using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace CatalogService.Infrastructure.Repositories;

public class TaxConfigurationRepository : ITaxConfigurationRepository
{
    private readonly CatalogDbContext _context;

    public TaxConfigurationRepository(CatalogDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<TaxConfiguration?> GetByIdAsync(TaxConfigId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.TaxConfigurations
            .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaxConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaxConfiguration>> FindAsync(
        Expression<Func<TaxConfiguration, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.TaxConfigurations.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TaxConfiguration?> FindFirstAsync(
        Expression<Func<TaxConfiguration, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.TaxConfigurations.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TaxConfiguration?> FindSingleAsync(
        Expression<Func<TaxConfiguration, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.TaxConfigurations.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<TaxConfiguration>> FindAsync(
        ISpecification<TaxConfiguration> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<TaxConfiguration?> FindFirstAsync(
        ISpecification<TaxConfiguration> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TaxConfiguration, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.TaxConfigurations.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<TaxConfiguration> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(TaxConfigId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.TaxConfigurations.AnyAsync(tc => tc.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<TaxConfiguration, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.TaxConfigurations.AnyAsync(predicate, cancellationToken);
    }

    public async Task<TaxConfiguration> AddAsync(TaxConfiguration entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.TaxConfigurations.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<TaxConfiguration>> AddRangeAsync(IEnumerable<TaxConfiguration> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.TaxConfigurations.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(TaxConfiguration entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.TaxConfigurations.Update(entity);
    }

    public void UpdateRange(IEnumerable<TaxConfiguration> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.TaxConfigurations.UpdateRange(entities);
    }

    public void Delete(TaxConfiguration entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.TaxConfigurations.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TaxConfiguration> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.TaxConfigurations.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(TaxConfigId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var taxConfig = await GetByIdAsync(id, cancellationToken);
        if (taxConfig is null) return false;
        
        Delete(taxConfig);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<TaxConfiguration>> GetByLocationIdAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations
            .Where(tc => tc.LocationId == locationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaxConfiguration?> GetByLocationAndCategoryAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations
            .FirstOrDefaultAsync(tc => tc.LocationId == locationId && tc.CategoryId == categoryId, cancellationToken);
    }

    public async Task<IEnumerable<TaxConfiguration>> GetApplicableConfigurationsAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations
            .Where(tc => tc.LocationId == locationId && 
                        (tc.CategoryId == null || tc.CategoryId == categoryId))
            .OrderByDescending(tc => tc.CategoryId != null) // Specific category rules first
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ConfigurationExistsAsync(int locationId, CategoryId? categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.TaxConfigurations
            .AnyAsync(tc => tc.LocationId == locationId && tc.CategoryId == categoryId, cancellationToken);
    }

    private IQueryable<TaxConfiguration> ApplySpecification(ISpecification<TaxConfiguration> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.TaxConfigurations.AsQueryable();

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