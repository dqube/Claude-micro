using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SharedService.Domain.Entities;
using SharedService.Domain.Repositories;
using SharedService.Domain.ValueObjects;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly SharedDbContext _context;

    public CountryRepository(SharedDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    // IReadOnlyRepository implementation
    public async Task<Country?> GetByIdAsync(CountryCode id, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> FindAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Country?> FindFirstAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Country?> FindSingleAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> FindAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<Country?> FindFirstAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency).Where(specification.Criteria).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CountryCode id, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Countries.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Country, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.CountAsync(specification.Criteria, cancellationToken);
    }

    // IRepository implementation  
    public async Task<Country> AddAsync(Country entity, CancellationToken cancellationToken = default)
    {
        _context.Countries.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Country>> AddRangeAsync(IEnumerable<Country> entities, CancellationToken cancellationToken = default)
    {
        _context.Countries.AddRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public void Update(Country entity)
    {
        _context.Countries.Update(entity);
    }

    public void UpdateRange(IEnumerable<Country> entities)
    {
        _context.Countries.UpdateRange(entities);
    }

    public async Task<Country> UpdateAsync(Country entity, CancellationToken cancellationToken = default)
    {
        _context.Countries.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Country>> UpdateRangeAsync(IEnumerable<Country> entities, CancellationToken cancellationToken = default)
    {
        _context.Countries.UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public void Delete(Country entity)
    {
        _context.Countries.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Country> entities)
    {
        _context.Countries.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(CountryCode id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;
        Delete(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Custom methods
    public async Task<Country?> GetByCodeAsync(CountryCode code, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(code, cancellationToken);
    }

    public async Task<IEnumerable<Country>> GetByCurrencyAsync(CurrencyCode currencyCode, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency)
            .Where(c => c.CurrencyCode == currencyCode)
            .OrderBy(c => c.Name.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Country>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Countries.Include(c => c.Currency)
            .OrderBy(c => c.Name.Value)
            .ToListAsync(cancellationToken);
    }
} 