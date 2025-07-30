using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories;

public class DiscountTypeRepository : IDiscountTypeRepository
{
    private readonly PromotionDbContext _context;

    public DiscountTypeRepository(PromotionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<DiscountType?> GetByIdAsync(DiscountTypeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountTypes
            .FirstOrDefaultAsync(dt => dt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountTypes.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountType>> FindAsync(
        Expression<Func<DiscountType, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountTypes.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<DiscountType?> FindFirstAsync(
        Expression<Func<DiscountType, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountTypes.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<DiscountType?> FindSingleAsync(
        Expression<Func<DiscountType, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountTypes.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountType>> FindAsync(
        ISpecification<DiscountType> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<DiscountType?> FindFirstAsync(
        ISpecification<DiscountType> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountTypes.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<DiscountType, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountTypes.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<DiscountType> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(DiscountTypeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountTypes.AnyAsync(dt => dt.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<DiscountType, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountTypes.AnyAsync(predicate, cancellationToken);
    }

    public async Task<DiscountType> AddAsync(DiscountType entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.DiscountTypes.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<DiscountType>> AddRangeAsync(IEnumerable<DiscountType> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.DiscountTypes.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(DiscountType entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountTypes.Update(entity);
    }

    public void UpdateRange(IEnumerable<DiscountType> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountTypes.UpdateRange(entities);
    }

    public void Delete(DiscountType entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountTypes.Remove(entity);
    }

    public void DeleteRange(IEnumerable<DiscountType> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountTypes.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(DiscountTypeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var discountType = await GetByIdAsync(id, cancellationToken);
        if (discountType is null) return false;
        
        Delete(discountType);
        return true;
    }

    // Domain-specific methods
    public async Task<DiscountType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.DiscountTypes
            .FirstOrDefaultAsync(dt => dt.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.DiscountTypes
            .AnyAsync(dt => dt.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<DiscountType>> GetAllOrderedByNameAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountTypes
            .OrderBy(dt => dt.Name)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<DiscountType> ApplySpecification(ISpecification<DiscountType> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.DiscountTypes.AsQueryable();

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