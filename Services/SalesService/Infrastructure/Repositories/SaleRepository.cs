using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.Repositories;
using SalesService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace SalesService.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _context;

    public SaleRepository(SalesDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(SaleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> FindAsync(
        Expression<Func<Sale, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Sales.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Sale?> FindFirstAsync(
        Expression<Func<Sale, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Sales.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Sale?> FindSingleAsync(
        Expression<Func<Sale, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Sales.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> FindAsync(
        ISpecification<Sale> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Sale?> FindFirstAsync(
        ISpecification<Sale> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Sale, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Sales.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Sale> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SaleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Sales.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Sale, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Sales.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Sale> AddAsync(Sale entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Sales.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Sale>> AddRangeAsync(IEnumerable<Sale> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Sales.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Sale entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Sales.Update(entity);
    }

    public void UpdateRange(IEnumerable<Sale> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Sales.UpdateRange(entities);
    }

    public void Delete(Sale entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Sales.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Sale> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Sales.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(SaleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale is null) return false;
        
        Delete(sale);
        return true;
    }

    public async Task<Sale?> GetByReceiptNumberAsync(ReceiptNumber receiptNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(receiptNumber);
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.ReceiptNumber == receiptNumber, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByStoreIdAsync(int storeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Where(s => s.StoreId == storeId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(s => s.TransactionTime >= fromDate.Value);
        }
        
        if (toDate.HasValue)
        {
            query = query.Where(s => s.TransactionTime <= toDate.Value);
        }
        
        return await query.OrderByDescending(s => s.TransactionTime).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByEmployeeIdAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Where(s => s.EmployeeId == employeeId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(s => s.TransactionTime >= fromDate.Value);
        }
        
        if (toDate.HasValue)
        {
            query = query.Where(s => s.TransactionTime <= toDate.Value);
        }
        
        return await query.OrderByDescending(s => s.TransactionTime).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.TransactionTime >= startDate && s.TransactionTime <= endDate)
            .OrderByDescending(s => s.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sale?> GetWithDetailsAsync(SaleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Sales
            .Include(s => s.SaleDetails)
            .Include(s => s.AppliedDiscounts)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    private IQueryable<Sale> ApplySpecification(ISpecification<Sale> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Sales.AsQueryable();

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