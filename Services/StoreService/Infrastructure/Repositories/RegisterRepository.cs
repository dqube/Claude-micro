using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;
using StoreService.Domain.Repositories;
using StoreService.Domain.ValueObjects;
using StoreService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace StoreService.Infrastructure.Repositories;

public class RegisterRepository : IRegisterRepository, IRepository<Register, RegisterId>, IReadOnlyRepository<Register, RegisterId>
{
    private readonly StoreDbContext _context;

    public RegisterRepository(StoreDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Register?> GetByIdAsync(RegisterId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Registers
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Register>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Registers.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Register>> GetByStoreIdAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);

        return await _context.Registers
            .Where(register => register.StoreId == storeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Register?> GetByStoreAndNameAsync(StoreId storeId, string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return await _context.Registers
            .FirstOrDefaultAsync(register => register.StoreId == storeId && register.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Register>> GetByStatusAsync(RegisterStatus status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(status);

        return await _context.Registers
            .Where(register => register.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Register>> GetOpenRegistersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Registers
            .Where(register => register.Status == RegisterStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Register>> GetOpenRegistersByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);

        return await _context.Registers
            .Where(register => register.StoreId == storeId && register.Status == RegisterStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithNameInStoreAsync(StoreId storeId, string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(storeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return await _context.Registers
            .AnyAsync(register => register.StoreId == storeId && register.Name == name, cancellationToken);
    }

    public async Task<Register> AddAsync(Register entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = await _context.Registers.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Register>> AddRangeAsync(IEnumerable<Register> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var entitiesList = entities.ToList();
        await _context.Registers.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public void Update(Register entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Registers.Update(entity);
    }

    public void UpdateRange(IEnumerable<Register> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Registers.UpdateRange(entities);
    }

    public void Delete(Register entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Registers.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Register> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _context.Registers.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(RegisterId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
            return true;
        }
        return false;
    }

    public async Task<IReadOnlyList<Register>> FindAsync(Expression<Func<Register, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Registers.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Register?> FindFirstAsync(Expression<Func<Register, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Registers.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Register?> FindSingleAsync(Expression<Func<Register, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Registers.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Register>> FindAsync(ISpecification<Register> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Registers.AsQueryable(), specification).ToListAsync(cancellationToken);
    }

    public async Task<Register?> FindFirstAsync(ISpecification<Register> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Registers.AsQueryable(), specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Registers.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Register, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Registers.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Register> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await SpecificationEvaluator.GetQuery(_context.Registers.AsQueryable(), specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(RegisterId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Registers.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Register, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _context.Registers.AnyAsync(predicate, cancellationToken);
    }
} 