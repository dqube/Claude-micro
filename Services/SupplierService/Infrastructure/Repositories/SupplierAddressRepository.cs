using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Repositories;
using SupplierService.Domain.ValueObjects;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class SupplierAddressRepository : ISupplierAddressRepository
{
    private readonly SupplierDbContext _context;

    public SupplierAddressRepository(SupplierDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<SupplierAddress?> GetByIdAsync(AddressId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.SupplierAddresses
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SupplierAddress>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierAddresses
            .Where(a => a.SupplierId == supplierId)
            .OrderBy(a => a.Line1)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupplierAddress?> GetPrimaryAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierAddresses
            .FirstOrDefaultAsync(a => a.SupplierId == supplierId && a.IsPrimary, cancellationToken);
    }

    public async Task<SupplierAddress?> GetShippingAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierAddresses
            .FirstOrDefaultAsync(a => a.SupplierId == supplierId && a.IsShipping, cancellationToken);
    }

    public async Task<SupplierAddress?> GetBillingAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierAddresses
            .FirstOrDefaultAsync(a => a.SupplierId == supplierId && a.IsBilling, cancellationToken);
    }

    public async Task<IEnumerable<SupplierAddress>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return Enumerable.Empty<SupplierAddress>();

        return await _context.SupplierAddresses
            .Where(a => a.CountryCode == countryCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SupplierAddress>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            return Enumerable.Empty<SupplierAddress>();

        return await _context.SupplierAddresses
            .Where(a => a.City.Contains(city))
            .ToListAsync(cancellationToken);
    }

    // Repository implementation
    public Task<SupplierAddress> AddAsync(SupplierAddress entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierAddresses.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<SupplierAddress>> AddRangeAsync(IEnumerable<SupplierAddress> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierAddresses.AddRange(entities);
        return Task.FromResult(entities);
    }

    public void Update(SupplierAddress entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierAddresses.Update(entity);
    }

    public void UpdateRange(IEnumerable<SupplierAddress> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierAddresses.UpdateRange(entities);
    }

    public void Delete(SupplierAddress entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierAddresses.Remove(entity);
    }

    public void DeleteRange(IEnumerable<SupplierAddress> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierAddresses.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(AddressId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<SupplierAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierAddress>> FindAsync(Expression<Func<SupplierAddress, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<SupplierAddress?> FindFirstAsync(Expression<Func<SupplierAddress, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<SupplierAddress?> FindSingleAsync(Expression<Func<SupplierAddress, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<SupplierAddress, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(AddressId id, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<SupplierAddress> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.AnyAsync(specification.Criteria, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<SupplierAddress, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<SupplierAddress> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierAddress>> FindAsync(ISpecification<SupplierAddress> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<SupplierAddress?> FindFirstAsync(ISpecification<SupplierAddress> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.FirstOrDefaultAsync(specification.Criteria, cancellationToken);
    }

    public async Task<SupplierAddress?> FindSingleAsync(ISpecification<SupplierAddress> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierAddresses.SingleOrDefaultAsync(specification.Criteria, cancellationToken);
    }
} 