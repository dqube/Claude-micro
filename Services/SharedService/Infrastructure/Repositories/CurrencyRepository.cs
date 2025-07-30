using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SharedService.Domain.Entities;
using SharedService.Domain.Repositories;
using SharedService.Domain.ValueObjects;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly SharedDbContext _context;

    public CurrencyRepository(SharedDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Currency?> GetByIdAsync(CurrencyCode id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Currencies.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Currency>> FindAsync(
        Expression<Func<Currency, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Currencies.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Currency?> FindFirstAsync(
        Expression<Func<Currency, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Currencies.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Currency?> FindSingleAsync(
        Expression<Func<Currency, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Currencies.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Currency>> FindAsync(
        ISpecification<Currency> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await _context.Currencies
            .Where(specification.Criteria)
            .ToListAsync(cancellationToken);
    }

    public async Task<Currency?> FindFirstAsync(
        ISpecification<Currency> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await _context.Currencies
            .Where(specification.Criteria)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CurrencyCode id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Currencies.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Currencies.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Currencies.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Currencies.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Currency> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await _context.Currencies.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<Currency> AddAsync(Currency entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Currencies.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Currency>> AddRangeAsync(IEnumerable<Currency> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Currencies.AddRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public void Update(Currency entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Currencies.Update(entity);
    }

    public void UpdateRange(IEnumerable<Currency> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Currencies.UpdateRange(entities);
    }

    public async Task<Currency> UpdateAsync(Currency entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Currencies.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Currency>> UpdateRangeAsync(IEnumerable<Currency> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Currencies.UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public void Delete(Currency entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Currencies.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Currency> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Currencies.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(CurrencyCode id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;
        Delete(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Custom methods
    public async Task<Currency?> GetByCodeAsync(CurrencyCode code, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(code, cancellationToken);
    }

    public async Task<IEnumerable<Currency>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Currencies
            .OrderBy(c => c.Name.Value)
            .ToListAsync(cancellationToken);
    }
} 