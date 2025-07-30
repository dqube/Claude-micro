using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Repositories;
using SupplierService.Domain.ValueObjects;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class SupplierContactRepository : ISupplierContactRepository
{
    private readonly SupplierDbContext _context;

    public SupplierContactRepository(SupplierDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<SupplierContact?> GetByIdAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.SupplierContacts
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SupplierContact>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierContacts
            .Where(c => c.SupplierId == supplierId)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupplierContact?> GetPrimaryContactAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.SupplierContacts
            .FirstOrDefaultAsync(c => c.SupplierId == supplierId && c.IsPrimary, cancellationToken);
    }

    public async Task<IEnumerable<SupplierContact>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Enumerable.Empty<SupplierContact>();

        return await _context.SupplierContacts
            .Where(c => c.Email == email)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupplierContact?> GetWithContactNumbersAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.SupplierContacts
            .Include(c => c.ContactNumbers)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    // Basic repository implementation methods
    public Task<SupplierContact> AddAsync(SupplierContact entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierContacts.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<SupplierContact>> AddRangeAsync(IEnumerable<SupplierContact> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierContacts.AddRange(entities);
        return Task.FromResult(entities);
    }

    public void Update(SupplierContact entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierContacts.Update(entity);
    }

    public void UpdateRange(IEnumerable<SupplierContact> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierContacts.UpdateRange(entities);
    }

    public void Delete(SupplierContact entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SupplierContacts.Remove(entity);
    }

    public void DeleteRange(IEnumerable<SupplierContact> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SupplierContacts.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<SupplierContact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierContact>> FindAsync(Expression<Func<SupplierContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<SupplierContact?> FindFirstAsync(Expression<Func<SupplierContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<SupplierContact?> FindSingleAsync(Expression<Func<SupplierContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<SupplierContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<SupplierContact> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.AnyAsync(specification.Criteria, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<SupplierContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<SupplierContact> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierContact>> FindAsync(ISpecification<SupplierContact> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<SupplierContact?> FindFirstAsync(ISpecification<SupplierContact> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.FirstOrDefaultAsync(specification.Criteria, cancellationToken);
    }

    public async Task<SupplierContact?> FindSingleAsync(ISpecification<SupplierContact> specification, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierContacts.SingleOrDefaultAsync(specification.Criteria, cancellationToken);
    }
} 