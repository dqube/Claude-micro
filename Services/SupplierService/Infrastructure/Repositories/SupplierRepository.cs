using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Repositories;
using SupplierService.Domain.ValueObjects;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly SupplierDbContext _context;

    public SupplierRepository(SupplierDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithContactsAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Suppliers
            .Include(s => s.Contacts)
            .ThenInclude(c => c.ContactNumbers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithAddressesAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Suppliers
            .Include(s => s.Addresses)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithPurchaseOrdersAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Suppliers
            .Include(s => s.PurchaseOrders)
            .ThenInclude(po => po.OrderDetails)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Supplier?> GetCompleteAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Suppliers
            .Include(s => s.Contacts)
            .ThenInclude(c => c.ContactNumbers)
            .Include(s => s.Addresses)
            .Include(s => s.PurchaseOrders)
            .ThenInclude(po => po.OrderDetails)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Enumerable.Empty<Supplier>();

        return await _context.Suppliers
            .Where(s => s.Name.Contains(name))
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return await _context.Suppliers
            .AnyAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            return Enumerable.Empty<Supplier>();

        return await _context.Suppliers
            .Where(s => s.TaxIdentificationNumber == taxId)
            .ToListAsync(cancellationToken);
    }

    // IRepository implementation - Async methods
    public Task<Supplier> AddAsync(Supplier entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Suppliers.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Supplier>> AddRangeAsync(IEnumerable<Supplier> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Suppliers.AddRange(entities);
        return Task.FromResult(entities);
    }

    public void Update(Supplier entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Suppliers.Update(entity);
    }

    public void UpdateRange(IEnumerable<Supplier> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Suppliers.UpdateRange(entities);
    }

    public void Delete(Supplier entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Suppliers.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Supplier> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Suppliers.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> FindAsync(Expression<Func<Supplier, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> FindFirstAsync(Expression<Func<Supplier, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Supplier?> FindSingleAsync(Expression<Func<Supplier, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Supplier, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(SupplierId id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<Supplier> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.AnyAsync(specification.Criteria, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Supplier, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Supplier> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> FindAsync(ISpecification<Supplier> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> FindFirstAsync(ISpecification<Supplier> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.FirstOrDefaultAsync(specification.Criteria, cancellationToken);
    }

    public async Task<Supplier?> FindSingleAsync(ISpecification<Supplier> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.SingleOrDefaultAsync(specification.Criteria, cancellationToken);
    }
} 