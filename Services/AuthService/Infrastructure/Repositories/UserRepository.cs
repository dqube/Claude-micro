using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> FindAsync(
        Expression<Func<User, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Users.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<User?> FindFirstAsync(
        Expression<Func<User, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Users.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<User?> FindSingleAsync(
        Expression<Func<User, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Users.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> FindAsync(
        ISpecification<User> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<User?> FindFirstAsync(
        ISpecification<User> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Users.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<User> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Users.AnyAsync(predicate, cancellationToken);
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Users.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Users.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Users.Update(entity);
    }

    public void UpdateRange(IEnumerable<User> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Users.UpdateRange(entities);
    }

    public void Delete(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Users.Remove(entity);
    }

    public void DeleteRange(IEnumerable<User> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Users.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var user = await GetByIdAsync(id, cancellationToken);
        if (user is null) return false;
        
        Delete(user);
        return true;
    }

    // Domain-specific methods
    public async Task<User?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(username);
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(username);
        return await _context.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetInactiveUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => !u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetLockedUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTime.UtcNow)
            .OrderBy(u => u.LockoutEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        
        return await _context.Users
            .Where(u => u.Username.Value.ToLower().Contains(lowerSearchTerm) ||
                       u.Email.Value.ToLower().Contains(lowerSearchTerm))
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetLockedOutUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTime.UtcNow)
            .OrderBy(u => u.LockoutEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(
        RoleId roleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roleId);
        return await _context.Users
            .Join(_context.UserRoles, u => u.Id, ur => ur.Id, (u, ur) => new { User = u, UserRole = ur })
            .Where(x => x.UserRole.RoleId == roleId)
            .Select(x => x.User)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<User> ApplySpecification(ISpecification<User> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.Users.AsQueryable();

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