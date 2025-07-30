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

public class StockMovementRepository : IStockMovementRepository
{
    private readonly InventoryDbContext _context;

    public StockMovementRepository(InventoryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<StockMovement?> GetByIdAsync(StockMovementId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.StockMovements
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> FindAsync(
        Expression<Func<StockMovement, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.StockMovements.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<StockMovement?> FindFirstAsync(
        Expression<Func<StockMovement, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.StockMovements.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<StockMovement?> FindSingleAsync(
        Expression<Func<StockMovement, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.StockMovements.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> FindAsync(
        ISpecification<StockMovement> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<StockMovement?> FindFirstAsync(
        ISpecification<StockMovement> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<StockMovement, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.StockMovements.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<StockMovement> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(StockMovementId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.StockMovements.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<StockMovement, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.StockMovements.AnyAsync(predicate, cancellationToken);
    }

    public async Task<StockMovement> AddAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.StockMovements.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<StockMovement>> AddRangeAsync(IEnumerable<StockMovement> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.StockMovements.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(StockMovement entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.StockMovements.Update(entity);
    }

    public void UpdateRange(IEnumerable<StockMovement> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.StockMovements.UpdateRange(entities);
    }

    public void Delete(StockMovement entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.StockMovements.Remove(entity);
    }

    public void DeleteRange(IEnumerable<StockMovement> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.StockMovements.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(StockMovementId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var movement = await GetByIdAsync(id, cancellationToken);
        if (movement is null) return false;
        
        Delete(movement);
        return true;
    }


    public async Task<IReadOnlyList<StockMovement>> GetByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.StockMovements
            .Where(x => x.StoreId == storeId)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<StockMovement>> GetByProductAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return await _context.StockMovements
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetByStoreAndProductAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        ArgumentNullException.ThrowIfNull(productId);
        return await _context.StockMovements
            .Where(x => x.StoreId == storeId && x.ProductId == productId)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Where(x => x.MovementDate >= startDate && x.MovementDate <= endDate)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetByMovementTypeAsync(MovementTypeValue movementType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(movementType);
        return await _context.StockMovements
            .Where(x => x.MovementType == movementType)
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<StockMovement> ApplySpecification(ISpecification<StockMovement> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.StockMovements.AsQueryable();

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