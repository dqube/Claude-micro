using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.Repositories;
using SalesService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace SalesService.Infrastructure.Repositories;

public class ReturnRepository : IReturnRepository
{
    private readonly SalesDbContext _context;

    public ReturnRepository(SalesDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Return?> GetByIdAsync(ReturnId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Returns
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Return>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Returns.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Return>> FindAsync(
        Expression<Func<Return, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Returns.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Return?> FindFirstAsync(
        Expression<Func<Return, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Returns.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Return?> FindSingleAsync(
        Expression<Func<Return, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Returns.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Return>> FindAsync(
        ISpecification<Return> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Return?> FindFirstAsync(
        ISpecification<Return> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Returns.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Return, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Returns.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Return> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(ReturnId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Returns.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Return, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Returns.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Return> AddAsync(Return entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Returns.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Return>> AddRangeAsync(IEnumerable<Return> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Returns.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Return entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Returns.Update(entity);
    }

    public void UpdateRange(IEnumerable<Return> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Returns.UpdateRange(entities);
    }

    public void Delete(Return entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Returns.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Return> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Returns.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(ReturnId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var returnEntity = await GetByIdAsync(id, cancellationToken);
        if (returnEntity is null) return false;
        
        Delete(returnEntity);
        return true;
    }

    public async Task<IEnumerable<Return>> GetBySaleIdAsync(SaleId saleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(saleId);
        return await _context.Returns
            .Where(r => r.SaleId == saleId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByEmployeeIdAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Returns.Where(r => r.EmployeeId == employeeId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(r => r.ReturnDate >= fromDate.Value);
        }
        
        if (toDate.HasValue)
        {
            query = query.Where(r => r.ReturnDate <= toDate.Value);
        }
        
        return await query.OrderByDescending(r => r.ReturnDate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Return?> GetWithDetailsAsync(ReturnId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Returns
            .Include(r => r.ReturnDetails)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    private IQueryable<Return> ApplySpecification(ISpecification<Return> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Returns.AsQueryable();

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