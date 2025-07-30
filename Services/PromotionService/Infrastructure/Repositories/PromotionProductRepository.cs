using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories;

public class PromotionProductRepository : IPromotionProductRepository
{
    private readonly PromotionDbContext _context;

    public PromotionProductRepository(PromotionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PromotionProduct?> GetByIdAsync(PromotionProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PromotionProducts
            .FirstOrDefaultAsync(pp => pp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionProduct>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PromotionProducts.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionProduct>> FindAsync(
        Expression<Func<PromotionProduct, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionProducts.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PromotionProduct?> FindFirstAsync(
        Expression<Func<PromotionProduct, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionProducts.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<PromotionProduct?> FindSingleAsync(
        Expression<Func<PromotionProduct, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionProducts.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionProduct>> FindAsync(
        ISpecification<PromotionProduct> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<PromotionProduct?> FindFirstAsync(
        ISpecification<PromotionProduct> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PromotionProducts.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<PromotionProduct, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionProducts.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<PromotionProduct> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PromotionProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PromotionProducts.AnyAsync(pp => pp.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<PromotionProduct, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionProducts.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PromotionProduct> AddAsync(PromotionProduct entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.PromotionProducts.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<PromotionProduct>> AddRangeAsync(IEnumerable<PromotionProduct> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.PromotionProducts.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(PromotionProduct entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PromotionProducts.Update(entity);
    }

    public void UpdateRange(IEnumerable<PromotionProduct> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PromotionProducts.UpdateRange(entities);
    }

    public void Delete(PromotionProduct entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PromotionProducts.Remove(entity);
    }

    public void DeleteRange(IEnumerable<PromotionProduct> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PromotionProducts.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(PromotionProductId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var promotionProduct = await GetByIdAsync(id, cancellationToken);
        if (promotionProduct is null) return false;
        
        Delete(promotionProduct);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<PromotionProduct>> GetByPromotionIdAsync(PromotionId promotionId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(promotionId);
        return await _context.PromotionProducts
            .Where(pp => pp.PromotionId == promotionId)
            .OrderBy(pp => pp.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<PromotionProduct> ApplySpecification(ISpecification<PromotionProduct> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.PromotionProducts.AsQueryable();

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