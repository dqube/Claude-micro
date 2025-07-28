using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;
using StoreService.Domain.Repositories;
using StoreService.Domain.ValueObjects;
using StoreService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace StoreService.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository, IRepository<Store, StoreId>, IReadOnlyRepository<Store, StoreId>
{
    private readonly StoreDbContext _context;

    public StoreRepository(StoreDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Store?> GetByIdAsync(StoreId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Store>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores.ToListAsync(cancellationToken);
    }

    public async Task<Store?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return await _context.Stores
            .FirstOrDefaultAsync(store => store.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetByStatusAsync(StoreStatus status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(status);

        return await _context.Stores
            .Where(store => store.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(store => store.LocationId == locationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetActiveStoresAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(store => store.Status == StoreStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return await _context.Stores
            .AnyAsync(store => store.Name == name, cancellationToken);
    }

    public async Task<Store> AddAsync(Store entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Stores.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Store>> AddRangeAsync(IEnumerable<Store> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var entitiesList = entities.ToList();
        await _context.Stores.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public void Update(Store entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Stores.Update(entity);
    }

    public void UpdateRange(IEnumerable<Store> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Stores.UpdateRange(entities);
    }

    public void Delete(Store entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Stores.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Store> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Stores.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(StoreId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<Store>> FindAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Stores.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Store?> FindFirstAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Stores.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Store?> FindSingleAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Stores.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Store>> FindAsync(ISpecification<Store> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Stores.AsQueryable(), specification).ToListAsync(cancellationToken);
    }

    public async Task<Store?> FindFirstAsync(ISpecification<Store> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Stores.AsQueryable(), specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Stores.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Store> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Stores.AsQueryable(), specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(StoreId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Stores.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Stores.AnyAsync(predicate, cancellationToken);
    }
} 