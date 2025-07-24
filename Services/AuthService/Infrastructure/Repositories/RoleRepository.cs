using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories;

public class RoleRepository : IRepository<Role, RoleId>, IReadOnlyRepository<Role, RoleId>
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> FindAsync(
        Expression<Func<Role, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Roles.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Role?> FindFirstAsync(
        Expression<Func<Role, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Roles.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Role?> FindSingleAsync(
        Expression<Func<Role, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Roles.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> FindAsync(
        ISpecification<Role> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Role?> FindFirstAsync(
        ISpecification<Role> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Role, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Roles.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Role> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Roles.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Role, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Roles.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Roles.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Role>> AddRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Roles.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Role entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Roles.Update(entity);
    }

    public void UpdateRange(IEnumerable<Role> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Roles.UpdateRange(entities);
    }

    public void Delete(Role entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Roles.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Role> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Roles.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var role = await GetByIdAsync(id, cancellationToken);
        if (role is null) return false;
        
        Delete(role);
        return true;
    }

    // Domain-specific methods
    public async Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _context.Roles
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetRolesByIdsAsync(
        IEnumerable<RoleId> roleIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleIds);
        return await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Role> ApplySpecification(ISpecification<Role> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Roles.AsQueryable();

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