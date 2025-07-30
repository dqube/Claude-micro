using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Repositories;
using SupplierService.Domain.ValueObjects;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly SupplierDbContext _context;

    public PurchaseOrderRepository(SupplierDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PurchaseOrders
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetWithDetailsAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PurchaseOrders
            .Include(po => po.OrderDetails)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        return await _context.PurchaseOrders
            .Where(po => po.SupplierId == supplierId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Where(po => po.StoreId == storeId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(status);
        return await _context.PurchaseOrders
            .Where(po => po.Status == status)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Where(po => po.Status == PurchaseOrderStatus.Ordered)
            .OrderBy(po => po.ExpectedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.PurchaseOrders
            .Where(po => po.Status == PurchaseOrderStatus.Ordered && po.ExpectedDate.HasValue && po.ExpectedDate.Value.Date < today)
            .OrderBy(po => po.ExpectedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAmountBySupplierAsync(SupplierId supplierId, DateTime? fromDate = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(supplierId);
        
        var query = _context.PurchaseOrders.Where(po => po.SupplierId == supplierId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(po => po.OrderDate >= fromDate.Value);
        }

        return await query.SumAsync(po => po.TotalAmount, cancellationToken);
    }

    // Repository implementation
    public Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrders.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<PurchaseOrder>> AddRangeAsync(IEnumerable<PurchaseOrder> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrders.AddRange(entities);
        return Task.FromResult(entities);
    }

    public void Update(PurchaseOrder entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrders.Update(entity);
    }

    public void UpdateRange(IEnumerable<PurchaseOrder> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrders.UpdateRange(entities);
    }

    public void Delete(PurchaseOrder entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrders.Remove(entity);
    }

    public void DeleteRange(IEnumerable<PurchaseOrder> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrders.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrder>> FindAsync(Expression<Func<PurchaseOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrder?> FindFirstAsync(Expression<Func<PurchaseOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<PurchaseOrder?> FindSingleAsync(Expression<Func<PurchaseOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<PurchaseOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.AnyAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<PurchaseOrder> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.AnyAsync(specification.Criteria, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<PurchaseOrder, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<PurchaseOrder> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrder>> FindAsync(ISpecification<PurchaseOrder> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrder?> FindFirstAsync(ISpecification<PurchaseOrder> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.FirstOrDefaultAsync(specification.Criteria, cancellationToken);
    }

    public async Task<PurchaseOrder?> FindSingleAsync(ISpecification<PurchaseOrder> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders.SingleOrDefaultAsync(specification.Criteria, cancellationToken);
    }
} 