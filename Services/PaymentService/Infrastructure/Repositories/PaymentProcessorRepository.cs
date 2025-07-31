using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentProcessorRepository : IPaymentProcessorRepository
{
    private readonly PaymentDbContext _context;

    public PaymentProcessorRepository(PaymentDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PaymentProcessor?> GetByIdAsync(PaymentProcessorId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PaymentProcessors
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentProcessor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProcessors.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentProcessor>> FindAsync(
        Expression<Func<PaymentProcessor, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PaymentProcessors.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PaymentProcessor?> FindFirstAsync(
        Expression<Func<PaymentProcessor, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PaymentProcessors.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<PaymentProcessor?> FindSingleAsync(
        Expression<Func<PaymentProcessor, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PaymentProcessors.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentProcessor>> FindAsync(
        ISpecification<PaymentProcessor> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<PaymentProcessor?> FindFirstAsync(
        ISpecification<PaymentProcessor> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProcessors.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<PaymentProcessor, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PaymentProcessors.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<PaymentProcessor> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PaymentProcessorId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.PaymentProcessors.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<PaymentProcessor, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.PaymentProcessors.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PaymentProcessor> AddAsync(PaymentProcessor entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.PaymentProcessors.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<PaymentProcessor>> AddRangeAsync(IEnumerable<PaymentProcessor> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.PaymentProcessors.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(PaymentProcessor entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PaymentProcessors.Update(entity);
    }

    public void UpdateRange(IEnumerable<PaymentProcessor> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PaymentProcessors.UpdateRange(entities);
    }

    public void Delete(PaymentProcessor entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.PaymentProcessors.Remove(entity);
    }

    public void DeleteRange(IEnumerable<PaymentProcessor> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.PaymentProcessors.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(PaymentProcessorId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var processor = await GetByIdAsync(id, cancellationToken);
        if (processor is null) return false;
        
        Delete(processor);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<PaymentProcessor>> GetActiveProcessorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProcessors
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentProcessor?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.PaymentProcessors
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.PaymentProcessors
            .AnyAsync(p => p.Name == name, cancellationToken);
    }

    private IQueryable<PaymentProcessor> ApplySpecification(ISpecification<PaymentProcessor> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.PaymentProcessors.AsQueryable();

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