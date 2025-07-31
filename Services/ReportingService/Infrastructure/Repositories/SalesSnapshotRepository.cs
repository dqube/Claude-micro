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

public class SalesSnapshotRepository : ISalesSnapshotRepository
{
    private readonly ReportingDbContext _context;

    public SalesSnapshotRepository(ReportingDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<SalesSnapshot?> GetByIdAsync(SalesSnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.SalesSnapshots
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SalesSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SalesSnapshots.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesSnapshot>> FindAsync(
        Expression<Func<SalesSnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.SalesSnapshots.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<SalesSnapshot?> FindFirstAsync(
        Expression<Func<SalesSnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.SalesSnapshots.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<SalesSnapshot?> FindSingleAsync(
        Expression<Func<SalesSnapshot, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.SalesSnapshots.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<SalesSnapshot>> FindAsync(
        ISpecification<SalesSnapshot> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<SalesSnapshot?> FindFirstAsync(
        ISpecification<SalesSnapshot> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SalesSnapshots.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<SalesSnapshot, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.SalesSnapshots.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<SalesSnapshot> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SalesSnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.SalesSnapshots.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<SalesSnapshot, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.SalesSnapshots.AnyAsync(predicate, cancellationToken);
    }

    public async Task<SalesSnapshot> AddAsync(SalesSnapshot entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.SalesSnapshots.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<SalesSnapshot>> AddRangeAsync(IEnumerable<SalesSnapshot> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.SalesSnapshots.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(SalesSnapshot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SalesSnapshots.Update(entity);
    }

    public void UpdateRange(IEnumerable<SalesSnapshot> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SalesSnapshots.UpdateRange(entities);
    }

    public void Delete(SalesSnapshot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.SalesSnapshots.Remove(entity);
    }

    public void DeleteRange(IEnumerable<SalesSnapshot> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.SalesSnapshots.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(SalesSnapshotId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var salesSnapshot = await GetByIdAsync(id, cancellationToken);
        if (salesSnapshot is null) return false;
        
        Delete(salesSnapshot);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<SalesSnapshot>> GetBySaleIdAsync(
        SaleId saleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(saleId);
        return await _context.SalesSnapshots
            .Where(s => s.SaleId == saleId)
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SalesSnapshot>> GetByStoreIdAsync(
        StoreId storeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.SalesSnapshots
            .Where(s => s.StoreId == storeId)
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SalesSnapshot>> GetByDateRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesSnapshots
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SalesSnapshot>> GetByStoreAndDateRangeAsync(
        StoreId storeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.SalesSnapshots
            .Where(s => s.StoreId == storeId && s.SaleDate >= startDate && s.SaleDate <= endDate)
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalSalesAmountAsync(
        StoreId storeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        return await _context.SalesSnapshots
            .Where(s => s.StoreId == storeId && s.SaleDate >= startDate && s.SaleDate <= endDate)
            .SumAsync(s => s.TotalAmount, cancellationToken);
    }

    public async Task<IEnumerable<SalesSnapshot>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        return await _context.SalesSnapshots
            .Where(s => s.CustomerId == customerId)
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<SalesSnapshot> ApplySpecification(ISpecification<SalesSnapshot> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.SalesSnapshots.AsQueryable();

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