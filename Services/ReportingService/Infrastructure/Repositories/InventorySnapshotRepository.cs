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

public class InventorySnapshotRepository : IInventorySnapshotRepository
{
    private readonly ReportingDbContext _context;

    public InventorySnapshotRepository(ReportingDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<InventorySnapshot?> GetByIdAsync(InventorySnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.InventorySnapshots
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventorySnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventorySnapshots.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventorySnapshot>> FindAsync(
        Expression<Func<InventorySnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventorySnapshots.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<InventorySnapshot?> FindFirstAsync(
        Expression<Func<InventorySnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventorySnapshots.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<InventorySnapshot?> FindSingleAsync(
        Expression<Func<InventorySnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventorySnapshots.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<InventorySnapshot>> FindAsync(
        ISpecification<InventorySnapshot> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<InventorySnapshot?> FindFirstAsync(
        ISpecification<InventorySnapshot> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventorySnapshots.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<InventorySnapshot, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventorySnapshots.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<InventorySnapshot> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(InventorySnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.InventorySnapshots.AnyAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<InventorySnapshot, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventorySnapshots.AnyAsync(predicate, cancellationToken);
    }

    public async Task<InventorySnapshot> AddAsync(InventorySnapshot entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.InventorySnapshots.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<InventorySnapshot>> AddRangeAsync(IEnumerable<InventorySnapshot> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.InventorySnapshots.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(InventorySnapshot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.InventorySnapshots.Update(entity);
    }

    public void UpdateRange(IEnumerable<InventorySnapshot> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.InventorySnapshots.UpdateRange(entities);
    }

    public void Delete(InventorySnapshot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.InventorySnapshots.Remove(entity);
    }

    public void DeleteRange(IEnumerable<InventorySnapshot> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.InventorySnapshots.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(InventorySnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var inventorySnapshot = await GetByIdAsync(id, cancellationToken);
        if (inventorySnapshot is null) return false;
        
        Delete(inventorySnapshot);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<InventorySnapshot>> GetByProductIdAsync(
        ProductId productId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return await _context.InventorySnapshots
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventorySnapshot>> GetByStoreIdAsync(
        StoreId storeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.InventorySnapshots
            .Where(i => i.StoreId == storeId)
            .OrderBy(i => i.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventorySnapshot>> GetByDateRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventorySnapshots
            .Where(i => i.SnapshotDate >= startDate && i.SnapshotDate <= endDate)
            .OrderBy(i => i.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventorySnapshot?> GetLatestByProductAndStoreAsync(
        ProductId productId,
        StoreId storeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentNullException.ThrowIfNull(storeId);
        
        return await _context.InventorySnapshots
            .Where(i => i.ProductId == productId && i.StoreId == storeId)
            .OrderByDescending(i => i.SnapshotDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventorySnapshot>> GetByProductStoreAndDateRangeAsync(
        ProductId productId,
        StoreId storeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentNullException.ThrowIfNull(storeId);
        
        return await _context.InventorySnapshots
            .Where(i => i.ProductId == productId && 
                       i.StoreId == storeId && 
                       i.SnapshotDate >= startDate && 
                       i.SnapshotDate <= endDate)
            .OrderBy(i => i.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventorySnapshot>> GetLowInventoryAsync(
        StoreId storeId,
        int threshold,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        
        // Get the latest snapshot for each product in the store that's below the threshold
        var latestSnapshots = await _context.InventorySnapshots
            .Where(i => i.StoreId == storeId)
            .GroupBy(i => i.ProductId)
            .Select(g => g.OrderByDescending(i => i.SnapshotDate).First())
            .Where(i => i.Quantity < threshold)
            .OrderBy(i => i.Quantity)
            .ToListAsync(cancellationToken);

        return latestSnapshots;
    }

    private IQueryable<InventorySnapshot> ApplySpecification(ISpecification<InventorySnapshot> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.InventorySnapshots.AsQueryable();

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