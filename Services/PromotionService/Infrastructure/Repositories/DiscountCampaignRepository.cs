using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories;

public class DiscountCampaignRepository : IDiscountCampaignRepository
{
    private readonly PromotionDbContext _context;

    public DiscountCampaignRepository(PromotionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<DiscountCampaign?> GetByIdAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountCampaigns
            .FirstOrDefaultAsync(dc => dc.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountCampaign>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountCampaigns.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountCampaign>> FindAsync(
        Expression<Func<DiscountCampaign, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountCampaigns.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<DiscountCampaign?> FindFirstAsync(
        Expression<Func<DiscountCampaign, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountCampaigns.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<DiscountCampaign?> FindSingleAsync(
        Expression<Func<DiscountCampaign, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountCampaigns.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<DiscountCampaign>> FindAsync(
        ISpecification<DiscountCampaign> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<DiscountCampaign?> FindFirstAsync(
        ISpecification<DiscountCampaign> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountCampaigns.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<DiscountCampaign, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountCampaigns.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<DiscountCampaign> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.DiscountCampaigns.AnyAsync(dc => dc.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<DiscountCampaign, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.DiscountCampaigns.AnyAsync(predicate, cancellationToken);
    }

    public async Task<DiscountCampaign> AddAsync(DiscountCampaign entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.DiscountCampaigns.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<DiscountCampaign>> AddRangeAsync(IEnumerable<DiscountCampaign> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.DiscountCampaigns.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(DiscountCampaign entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountCampaigns.Update(entity);
    }

    public void UpdateRange(IEnumerable<DiscountCampaign> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountCampaigns.UpdateRange(entities);
    }

    public void Delete(DiscountCampaign entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.DiscountCampaigns.Remove(entity);
    }

    public void DeleteRange(IEnumerable<DiscountCampaign> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.DiscountCampaigns.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(CampaignId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var campaign = await GetByIdAsync(id, cancellationToken);
        if (campaign is null) return false;
        
        Delete(campaign);
        return true;
    }

    // Domain-specific methods
    public async Task<DiscountCampaign?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.DiscountCampaigns
            .FirstOrDefaultAsync(dc => dc.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<DiscountCampaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DiscountCampaigns
            .Where(dc => dc.IsActive)
            .OrderBy(dc => dc.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCampaign>> GetCampaignsInDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.DiscountCampaigns
            .Where(dc => dc.StartDate <= endDate && dc.EndDate >= startDate)
            .OrderBy(dc => dc.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCampaign>> GetCampaignsEndingSoonAsync(int days, CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow.AddDays(days);
        return await _context.DiscountCampaigns
            .Where(dc => dc.IsActive && dc.EndDate <= endDate && dc.EndDate >= DateTime.UtcNow)
            .OrderBy(dc => dc.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.DiscountCampaigns
            .AnyAsync(dc => dc.Name == name, cancellationToken);
    }

    public async Task<DiscountCampaign?> GetWithRulesAsync(CampaignId campaignId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaignId);
        return await _context.DiscountCampaigns
            .Include(dc => dc.Rules)
            .FirstOrDefaultAsync(dc => dc.Id == campaignId, cancellationToken);
    }

    private IQueryable<DiscountCampaign> ApplySpecification(ISpecification<DiscountCampaign> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.DiscountCampaigns.AsQueryable();

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