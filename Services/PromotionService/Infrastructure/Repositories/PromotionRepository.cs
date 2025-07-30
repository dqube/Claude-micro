using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly PromotionDbContext _context;

    public PromotionRepository(PromotionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Promotion?> GetByIdAsync(PromotionId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Promotions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Promotions.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Promotion>> FindAsync(
        Expression<Func<Promotion, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Promotions.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Promotion?> FindFirstAsync(
        Expression<Func<Promotion, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Promotions.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Promotion?> FindSingleAsync(
        Expression<Func<Promotion, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Promotions.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Promotion>> FindAsync(
        ISpecification<Promotion> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Promotion?> FindFirstAsync(
        ISpecification<Promotion> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Promotions.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Promotion, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Promotions.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Promotion> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PromotionId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Promotions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Promotion, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Promotions.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Promotion> AddAsync(Promotion entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Promotions.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Promotion>> AddRangeAsync(IEnumerable<Promotion> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Promotions.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Promotion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Promotions.Update(entity);
    }

    public void UpdateRange(IEnumerable<Promotion> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Promotions.UpdateRange(entities);
    }

    public void Delete(Promotion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Promotions.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Promotion> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Promotions.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(PromotionId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var promotion = await GetByIdAsync(id, cancellationToken);
        if (promotion is null) return false;
        
        Delete(promotion);
        return true;
    }

    // Domain-specific methods
    public async Task<Promotion?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.Promotions
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        return await _context.Promotions
            .Where(p => p.StartDate <= currentDate && p.EndDate >= currentDate)
            .OrderBy(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Promotion>> GetPromotionsInDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Promotions
            .Where(p => p.StartDate <= endDate && p.EndDate >= startDate)
            .OrderBy(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Promotion>> GetCombinablePromotionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Promotions
            .Where(p => p.IsCombinable)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Promotion>> GetPromotionsEndingSoonAsync(int days, CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow.AddDays(days);
        return await _context.Promotions
            .Where(p => p.EndDate <= endDate && p.EndDate >= DateTime.UtcNow)
            .OrderBy(p => p.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.Promotions
            .AnyAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<Promotion?> GetWithProductsAsync(PromotionId promotionId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(promotionId);
        return await _context.Promotions
            .Include(p => p.PromotionProducts)
            .FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);
    }

    private IQueryable<Promotion> ApplySpecification(ISpecification<Promotion> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Promotions.AsQueryable();

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