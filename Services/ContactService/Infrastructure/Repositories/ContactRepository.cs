using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using ContactService.Domain.Entities;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using ContactService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ContactService.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ContactDbContext _context;

    public ContactRepository(ContactDbContext context)
    {
        _context = context;
    }

    // IContactRepository specific methods
    public async Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .FirstOrDefaultAsync(c => c.Email!.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<Contact>> GetByContactTypeAsync(ContactType contactType, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .Where(c => c.ContactType!.Name == contactType.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Contact>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .Where(c => c.FirstName.Contains(searchTerm) || 
                       c.LastName.Contains(searchTerm) ||
                       (c.Company != null && c.Company.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts
            .AnyAsync(c => c.Email!.Value == email, cancellationToken);
    }

    // IReadOnlyRepository<Contact, ContactId> implementation
    public async Task<Contact?> GetByIdAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var contacts = await _context.Contacts.ToListAsync(cancellationToken);
        return contacts.AsReadOnly();
    }

    public async Task<IReadOnlyList<Contact>> FindAsync(Expression<Func<Contact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var contacts = await _context.Contacts.Where(predicate).ToListAsync(cancellationToken);
        return contacts.AsReadOnly();
    }

    public async Task<Contact?> FindFirstAsync(Expression<Func<Contact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Contact?> FindSingleAsync(Expression<Func<Contact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> FindAsync(ISpecification<Contact> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var contacts = await query.ToListAsync(cancellationToken);
        return contacts.AsReadOnly();
    }

    public async Task<Contact?> FindFirstAsync(ISpecification<Contact> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Contact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Contact> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Contact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Contacts.AnyAsync(predicate, cancellationToken);
    }

    // IRepository<Contact, ContactId> implementation
    public async Task<Contact> AddAsync(Contact entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Contacts.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Contact>> AddRangeAsync(IEnumerable<Contact> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        await _context.Contacts.AddRangeAsync(entityList, cancellationToken);
        return entityList;
    }

    public void Update(Contact entity)
    {
        _context.Contacts.Update(entity);
    }

    public void UpdateRange(IEnumerable<Contact> entities)
    {
        _context.Contacts.UpdateRange(entities);
    }

    public void Delete(Contact entity)
    {
        _context.Contacts.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Contact> entities)
    {
        _context.Contacts.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(ContactId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    private IQueryable<Contact> ApplySpecification(ISpecification<Contact> specification)
    {
        var query = _context.Contacts.AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(g => g);
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}