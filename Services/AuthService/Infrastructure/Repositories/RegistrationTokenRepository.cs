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

public class RegistrationTokenRepository : IRegistrationTokenRepository
{
    private readonly AuthDbContext _context;

    public RegistrationTokenRepository(AuthDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<RegistrationToken?> GetByIdAsync(TokenId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.RegistrationTokens
            .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RegistrationToken>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RegistrationTokens.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistrationToken>> FindAsync(
        Expression<Func<RegistrationToken, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.RegistrationTokens.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<RegistrationToken?> FindFirstAsync(
        Expression<Func<RegistrationToken, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.RegistrationTokens.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<RegistrationToken?> FindSingleAsync(
        Expression<Func<RegistrationToken, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.RegistrationTokens.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<RegistrationToken>> FindAsync(
        ISpecification<RegistrationToken> specification, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<RegistrationToken?> FindFirstAsync(
        ISpecification<RegistrationToken> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RegistrationTokens.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<RegistrationToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.RegistrationTokens.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<RegistrationToken> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(TokenId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.RegistrationTokens.AnyAsync(rt => rt.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<RegistrationToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.RegistrationTokens.AnyAsync(predicate, cancellationToken);
    }

    public async Task<RegistrationToken> AddAsync(RegistrationToken entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.RegistrationTokens.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<RegistrationToken>> AddRangeAsync(IEnumerable<RegistrationToken> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.RegistrationTokens.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(RegistrationToken entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.RegistrationTokens.Update(entity);
    }

    public void UpdateRange(IEnumerable<RegistrationToken> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.RegistrationTokens.UpdateRange(entities);
    }

    public void Delete(RegistrationToken entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.RegistrationTokens.Remove(entity);
    }

    public void DeleteRange(IEnumerable<RegistrationToken> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.RegistrationTokens.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(TokenId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var token = await GetByIdAsync(id, cancellationToken);
        if (token is null) return false;
        
        Delete(token);
        return true;
    }

    // Domain-specific methods
    public async Task<RegistrationToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        return await _context.RegistrationTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<RegistrationToken?> GetValidTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        var now = DateTime.UtcNow;
        return await _context.RegistrationTokens
            .FirstOrDefaultAsync(rt => rt.Token == token && 
                                     !rt.IsUsed && 
                                     rt.Expiration > now, 
                                cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetTokensByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        return await _context.RegistrationTokens
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetTokensByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _context.RegistrationTokens
            .Where(rt => rt.Email == email)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetTokensByTypeAsync(
        TokenType tokenType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tokenType);
        return await _context.RegistrationTokens
            .Where(rt => rt.TokenType == tokenType)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetExpiredTokensAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.RegistrationTokens
            .Where(rt => !rt.IsUsed && rt.Expiration <= now)
            .OrderBy(rt => rt.Expiration)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RegistrationToken>> GetUnusedTokensAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.RegistrationTokens
            .Where(rt => !rt.IsUsed)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<RegistrationToken?> GetLatestValidTokenAsync(
        UserId userId,
        TokenType tokenType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(tokenType);
        
        var now = DateTime.UtcNow;
        return await _context.RegistrationTokens
            .Where(rt => rt.UserId == userId && 
                        rt.TokenType == tokenType && 
                        !rt.IsUsed && 
                        rt.Expiration > now)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CleanExpiredTokensAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredTokens = await _context.RegistrationTokens
            .Where(rt => !rt.IsUsed && rt.Expiration <= now)
            .ToListAsync(cancellationToken);
            
        if (expiredTokens.Count == 0) return 0;
        
        _context.RegistrationTokens.RemoveRange(expiredTokens);
        return expiredTokens.Count;
    }

    private IQueryable<RegistrationToken> ApplySpecification(ISpecification<RegistrationToken> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        var query = _context.RegistrationTokens.AsQueryable();

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