using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories;

public class DiscountRuleRepository : IDiscountRuleRepository
{
    private readonly PromotionDbContext _context;

    public DiscountRuleRepository(PromotionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<DiscountRule?> GetByIdAsync(RuleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountRules
            .FirstOrDefaultAsync(dr => dr.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountRules.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountRule>> FindAsync(
        Expression<Func<DiscountRule, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountRules.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<DiscountRule?> FindFirstAsync(
        Expression<Func<DiscountRule, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountRules.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<DiscountRule?> FindSingleAsync(
        Expression<Func<DiscountRule, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountRules.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountRule>> FindAsync(
        ISpecification<DiscountRule> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<DiscountRule?> FindFirstAsync(
        ISpecification<DiscountRule> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountRules.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<DiscountRule, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountRules.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<DiscountRule> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RuleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountRules.AnyAsync(dr => dr.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<DiscountRule, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountRules.AnyAsync(predicate, cancellationToken);
    }

    public async Task<DiscountRule> AddAsync(DiscountRule entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.DiscountRules.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<DiscountRule>> AddRangeAsync(IEnumerable<DiscountRule> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.DiscountRules.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(DiscountRule entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountRules.Update(entity);
    }

    public void UpdateRange(IEnumerable<DiscountRule> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountRules.UpdateRange(entities);
    }

    public void Delete(DiscountRule entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountRules.Remove(entity);
    }

    public void DeleteRange(IEnumerable<DiscountRule> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountRules.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(RuleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var rule = await GetByIdAsync(id, cancellationToken);
        if (rule is null) return false;
        
        Delete(rule);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<DiscountRule>> GetByCampaignIdAsync(CampaignId campaignId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaignId);
        return await _context.DiscountRules
            .Where(dr => dr.CampaignId == campaignId)
            .OrderBy(dr => dr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<DiscountRule> ApplySpecification(ISpecification<DiscountRule> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.DiscountRules.AsQueryable();

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