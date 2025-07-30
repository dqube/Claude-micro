using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly InventoryDbContext _context;

    public InventoryItemRepository(InventoryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<InventoryItem?> GetByIdAsync(InventoryItemId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> FindAsync(
        Expression<Func<InventoryItem, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventoryItems.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> FindFirstAsync(
        Expression<Func<InventoryItem, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventoryItems.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<InventoryItem?> FindSingleAsync(
        Expression<Func<InventoryItem, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventoryItems.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> FindAsync(
        ISpecification<InventoryItem> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> FindFirstAsync(
        ISpecification<InventoryItem> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<InventoryItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventoryItems.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<InventoryItem> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(InventoryItemId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.InventoryItems.AnyAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<InventoryItem, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.InventoryItems.AnyAsync(predicate, cancellationToken);
    }

    public async Task<InventoryItem> AddAsync(InventoryItem entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.InventoryItems.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<InventoryItem>> AddRangeAsync(IEnumerable<InventoryItem> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.InventoryItems.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(InventoryItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.InventoryItems.Update(entity);
    }

    public void UpdateRange(IEnumerable<InventoryItem> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.InventoryItems.UpdateRange(entities);
    }

    public void Delete(InventoryItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.InventoryItems.Remove(entity);
    }

    public void DeleteRange(IEnumerable<InventoryItem> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.InventoryItems.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(InventoryItemId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var item = await GetByIdAsync(id, cancellationToken);
        if (item is null) return false;
        
        Delete(item);
        return true;
    }

    public async Task<InventoryItem?> GetByStoreAndProductAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        ArgumentNullException.ThrowIfNull(productId);
        return await _context.InventoryItems
            .FirstOrDefaultAsync(x => x.StoreId == storeId && x.ProductId == productId, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.InventoryItems
            .Where(x => x.StoreId == storeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.InventoryItems
            .Where(x => x.StoreId == storeId && x.Quantity <= x.ReorderLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetOutOfStockItemsAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.InventoryItems
            .Where(x => x.StoreId == storeId && x.Quantity == 0)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        ArgumentNullException.ThrowIfNull(productId);
        return await _context.InventoryItems
            .AnyAsync(x => x.StoreId == storeId && x.ProductId == productId, cancellationToken);
    }

    private IQueryable<InventoryItem> ApplySpecification(ISpecification<InventoryItem> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.InventoryItems.AsQueryable();

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