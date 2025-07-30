using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Repositories;
using SupplierService.Domain.ValueObjects;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
{
    private readonly SupplierDbContext _context;

    public PurchaseOrderDetailRepository(SupplierDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PurchaseOrderDetail?> GetByIdAsync(OrderDetailId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PurchaseOrderDetails
            .FirstOrDefaultAsync(pod => pod.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderId);
        return await _context.PurchaseOrderDetails
            .Where(pod => pod.OrderId == orderId)
            .OrderBy(pod => pod.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails
            .Where(pod => pod.ProductId == productId)
            .OrderByDescending(pod => pod.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetPendingReceiptsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails
            .Where(pod => pod.ReceivedQuantity == null || pod.ReceivedQuantity < pod.Quantity)
            .OrderBy(pod => pod.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetPartiallyReceivedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails
            .Where(pod => pod.ReceivedQuantity != null && pod.ReceivedQuantity > 0 && pod.ReceivedQuantity < pod.Quantity)
            .OrderBy(pod => pod.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalQuantityByProductAsync(Guid productId, DateTime? fromDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PurchaseOrderDetails.Where(pod => pod.ProductId == productId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(pod => pod.CreatedAt >= fromDate.Value);
        }

        return await query.SumAsync(pod => pod.Quantity, cancellationToken);
    }

    // Repository implementation
    public Task<PurchaseOrderDetail> AddAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrderDetails.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<PurchaseOrderDetail>> AddRangeAsync(IEnumerable<PurchaseOrderDetail> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrderDetails.AddRange(entities);
        return Task.FromResult(entities);
    }

    public void Update(PurchaseOrderDetail entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrderDetails.Update(entity);
    }

    public void UpdateRange(IEnumerable<PurchaseOrderDetail> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrderDetails.UpdateRange(entities);
    }

    public void Delete(PurchaseOrderDetail entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PurchaseOrderDetails.Remove(entity);
    }

    public void DeleteRange(IEnumerable<PurchaseOrderDetail> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PurchaseOrderDetails.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(OrderDetailId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<PurchaseOrderDetail>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrderDetail>> FindAsync(Expression<Func<PurchaseOrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrderDetail?> FindFirstAsync(Expression<Func<PurchaseOrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<PurchaseOrderDetail?> FindSingleAsync(Expression<Func<PurchaseOrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<PurchaseOrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(OrderDetailId id, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.AnyAsync(pod => pod.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ISpecification<PurchaseOrderDetail> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.AnyAsync(specification.Criteria, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<PurchaseOrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<PurchaseOrderDetail> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.CountAsync(specification.Criteria, cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseOrderDetail>> FindAsync(ISpecification<PurchaseOrderDetail> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.Where(specification.Criteria).ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrderDetail?> FindFirstAsync(ISpecification<PurchaseOrderDetail> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.FirstOrDefaultAsync(specification.Criteria, cancellationToken);
    }

    public async Task<PurchaseOrderDetail?> FindSingleAsync(ISpecification<PurchaseOrderDetail> specification, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderDetails.SingleOrDefaultAsync(specification.Criteria, cancellationToken);
    }
} 