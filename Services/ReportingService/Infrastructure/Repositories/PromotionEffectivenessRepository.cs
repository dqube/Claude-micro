using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;
using ReportingService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace ReportingService.Infrastructure.Repositories;

public class PromotionEffectivenessRepository : IPromotionEffectivenessRepository
{
    private readonly ReportingDbContext _context;

    public PromotionEffectivenessRepository(ReportingDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PromotionEffectiveness?> GetByIdAsync(PromotionEffectivenessId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PromotionEffectiveness
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionEffectiveness>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionEffectiveness>> FindAsync(
        Expression<Func<PromotionEffectiveness, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionEffectiveness.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PromotionEffectiveness?> FindFirstAsync(
        Expression<Func<PromotionEffectiveness, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionEffectiveness.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<PromotionEffectiveness?> FindSingleAsync(
        Expression<Func<PromotionEffectiveness, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionEffectiveness.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionEffectiveness>> FindAsync(
        ISpecification<PromotionEffectiveness> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<PromotionEffectiveness?> FindFirstAsync(
        ISpecification<PromotionEffectiveness> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<PromotionEffectiveness, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionEffectiveness.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<PromotionEffectiveness> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PromotionEffectivenessId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PromotionEffectiveness.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<PromotionEffectiveness, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PromotionEffectiveness.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PromotionEffectiveness> AddAsync(PromotionEffectiveness entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.PromotionEffectiveness.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<PromotionEffectiveness>> AddRangeAsync(IEnumerable<PromotionEffectiveness> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.PromotionEffectiveness.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(PromotionEffectiveness entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PromotionEffectiveness.Update(entity);
    }

    public void UpdateRange(IEnumerable<PromotionEffectiveness> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PromotionEffectiveness.UpdateRange(entities);
    }

    public void Delete(PromotionEffectiveness entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PromotionEffectiveness.Remove(entity);
    }

    public void DeleteRange(IEnumerable<PromotionEffectiveness> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PromotionEffectiveness.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(PromotionEffectivenessId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var promotionEffectiveness = await GetByIdAsync(id, cancellationToken);
        if (promotionEffectiveness is null) return false;
        
        Delete(promotionEffectiveness);
        return true;
    }

    // Domain-specific methods
    public async Task<PromotionEffectiveness?> GetByPromotionIdAsync(
        PromotionId promotionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(promotionId);
        return await _context.PromotionEffectiveness
            .FirstOrDefaultAsync(p => p.PromotionId == promotionId, cancellationToken);
    }

    public async Task<IEnumerable<PromotionEffectiveness>> GetByDateRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness
            .Where(p => p.AnalysisDate >= startDate && p.AnalysisDate <= endDate)
            .OrderBy(p => p.AnalysisDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PromotionEffectiveness>> GetTopByRedemptionCountAsync(
        int topCount,
        CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness
            .OrderByDescending(p => p.RedemptionCount)
            .Take(topCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PromotionEffectiveness>> GetTopByRevenueImpactAsync(
        int topCount,
        CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness
            .OrderByDescending(p => p.RevenueImpact)
            .Take(topCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PromotionEffectiveness>> GetUnderperformingPromotionsAsync(
        int redemptionThreshold,
        decimal revenueThreshold,
        CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness
            .Where(p => p.RedemptionCount < redemptionThreshold || p.RevenueImpact < revenueThreshold)
            .OrderBy(p => p.RedemptionCount)
            .ThenBy(p => p.RevenueImpact)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetAverageRedemptionCountAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var promotions = await _context.PromotionEffectiveness
            .Where(p => p.AnalysisDate >= startDate && p.AnalysisDate <= endDate)
            .ToListAsync(cancellationToken);

        return promotions.Count > 0 ? promotions.Average(p => p.RedemptionCount) : 0;
    }

    public async Task<decimal> GetTotalRevenueImpactAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.PromotionEffectiveness
            .Where(p => p.AnalysisDate >= startDate && p.AnalysisDate <= endDate)
            .SumAsync(p => p.RevenueImpact, cancellationToken);
    }

    private IQueryable<PromotionEffectiveness> ApplySpecification(ISpecification<PromotionEffectiveness> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.PromotionEffectiveness.AsQueryable();

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