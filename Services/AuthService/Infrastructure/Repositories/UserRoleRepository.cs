using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AuthDbContext _context;

    public UserRoleRepository(AuthDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<UserRole?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> FindAsync(
        Expression<Func<UserRole, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.UserRoles.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> FindFirstAsync(
        Expression<Func<UserRole, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.UserRoles.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<UserRole?> FindSingleAsync(
        Expression<Func<UserRole, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.UserRoles.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> FindAsync(
        ISpecification<UserRole> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> FindFirstAsync(
        ISpecification<UserRole> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<UserRole, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.UserRoles.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<UserRole> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.UserRoles.AnyAsync(ur => ur.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<UserRole, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.UserRoles.AnyAsync(predicate, cancellationToken);
    }

    public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.UserRoles.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<UserRole>> AddRangeAsync(IEnumerable<UserRole> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.UserRoles.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(UserRole entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.UserRoles.Update(entity);
    }

    public void UpdateRange(IEnumerable<UserRole> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.UserRoles.UpdateRange(entities);
    }

    public void Delete(UserRole entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.UserRoles.Remove(entity);
    }

    public void DeleteRange(IEnumerable<UserRole> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.UserRoles.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var userRole = await GetByIdAsync(id, cancellationToken);
        if (userRole is null) return false;
        
        Delete(userRole);
        return true;
    }

    // Domain-specific methods
    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        return await _context.UserRoles
            .Where(ur => ur.Id == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetByUserIdAndRoleIdAsync(UserId userId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.Id == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        return await _context.UserRoles
            .Where(ur => ur.Id == userId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Join(_context.Users, ur => ur.Id, u => u.Id, (ur, u) => u)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(UserId userId, RoleId roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(roleId);
        return await _context.UserRoles
            .AnyAsync(ur => ur.Id == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task<int> RemoveAllRolesFromUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var userRoles = await _context.UserRoles
            .Where(ur => ur.Id == userId)
            .ToListAsync(cancellationToken);
        
        if (userRoles.Count == 0) return 0;
        
        _context.UserRoles.RemoveRange(userRoles);
        return userRoles.Count;
    }

    public async Task<int> RemoveAllUsersFromRoleAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        var userRoles = await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync(cancellationToken);
        
        if (userRoles.Count == 0) return 0;
        
        _context.UserRoles.RemoveRange(userRoles);
        return userRoles.Count;
    }

    private IQueryable<UserRole> ApplySpecification(ISpecification<UserRole> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.UserRoles.AsQueryable();

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