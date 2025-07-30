using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.ValueObjects;
using EmployeeService.Domain.Repositories;
using EmployeeService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace EmployeeService.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _context;

    public EmployeeRepository(EmployeeDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(EmployeeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> FindAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> FindFirstAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Employee?> FindSingleAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> FindAsync(
        ISpecification<Employee> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Employee?> FindFirstAsync(
        ISpecification<Employee> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Employees.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Employee> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(EmployeeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Employees.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Employees.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Employees.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Employee>> AddRangeAsync(IEnumerable<Employee> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Employees.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Employees.Update(entity);
    }

    public void UpdateRange(IEnumerable<Employee> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Employees.UpdateRange(entities);
    }

    public void Delete(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Employees.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Employee> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Employees.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(EmployeeId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var employee = await GetByIdAsync(id, cancellationToken);
        if (employee is null) return false;
        
        Delete(employee);
        return true;
    }

    // Domain-specific methods
    public async Task<Employee?> GetByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employeeNumber);
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber, cancellationToken);
    }

    public async Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .Where(e => e.StoreId == storeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employeeNumber);
        return await _context.Employees
            .AnyAsync(e => e.EmployeeNumber == employeeNumber, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(e => e.UserId == userId, cancellationToken);
    }

    private IQueryable<Employee> ApplySpecification(ISpecification<Employee> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Employees
            .Include(e => e.ContactNumbers)
            .Include(e => e.Addresses)
            .AsQueryable();

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