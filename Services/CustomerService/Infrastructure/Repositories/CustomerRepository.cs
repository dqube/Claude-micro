using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace CustomerService.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    // IReadOnlyRepository implementation
    public async Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Customers.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Customer?> FindFirstAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Customers.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Customer?> FindSingleAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Customers.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> FindAsync(ISpecification<Customer> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Customer?> FindFirstAsync(ISpecification<Customer> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Customers.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Customer> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Customers.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await CountAsync(cancellationToken);
        var customers = await _context.Customers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(customers, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(Expression<Func<Customer, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        var totalCount = await CountAsync(predicate, cancellationToken);
        var customers = await _context.Customers
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(customers, totalCount, pageNumber, pageSize);
    }

    // IRepository implementation
    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Customers.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Customer>> AddRangeAsync(IEnumerable<Customer> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Customers.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Customer entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Customers.Update(entity);
    }

    public void UpdateRange(IEnumerable<Customer> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Customers.UpdateRange(entities);
    }

    public void Delete(Customer entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Customers.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Customer> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Customers.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer is null) return false;
        
        Delete(customer);
        return true;
    }

    // Domain-specific methods
    public async Task<Customer?> GetByMembershipNumberAsync(MembershipNumber membershipNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(membershipNumber);
        return await _context.Customers.FirstOrDefaultAsync(c => c.MembershipNumber == membershipNumber, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _context.Customers.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> MembershipNumberExistsAsync(MembershipNumber membershipNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(membershipNumber);
        return await _context.Customers.AnyAsync(c => c.MembershipNumber == membershipNumber, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithExpiredMembershipsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Customers
            .Where(c => c.ExpiryDate < today)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetCustomersByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(countryCode);
        return await _context.Customers
            .Where(c => c.CountryCode == countryCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetCustomerWithContactsAndAddressesAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        return await _context.Customers
            .Include(c => c.ContactNumbers)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
    }

    public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetCustomersPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? countryCode = null, 
        bool? isMembershipActive = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Customers
            .Include(c => c.ContactNumbers)
            .Include(c => c.Addresses)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(c => 
                c.FirstName.ToLower().Contains(searchLower) ||
                c.LastName.ToLower().Contains(searchLower) ||
                (c.Email != null && c.Email.Value.ToLower().Contains(searchLower)) ||
                c.MembershipNumber.Value.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            query = query.Where(c => c.CountryCode == countryCode);
        }

        if (isMembershipActive.HasValue)
        {
            var today = DateTime.UtcNow.Date;
            if (isMembershipActive.Value)
            {
                query = query.Where(c => c.ExpiryDate >= today);
            }
            else
            {
                query = query.Where(c => c.ExpiryDate < today);
            }
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var customers = await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }

    private IQueryable<Customer> ApplySpecification(ISpecification<Customer> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Customers.AsQueryable();

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