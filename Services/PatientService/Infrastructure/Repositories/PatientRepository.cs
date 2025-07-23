using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PatientService.Infrastructure.Repositories;

public class PatientRepository : IRepository<Patient, PatientId>, IReadOnlyRepository<Patient, PatientId>
{
    private readonly PatientDbContext _context;

    public PatientRepository(PatientDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> FindAsync(
        Expression<Func<Patient, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Patient?> FindFirstAsync(
        Expression<Func<Patient, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Patient?> FindSingleAsync(
        Expression<Func<Patient, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> FindAsync(
        ISpecification<Patient> specification, 
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public async Task<Patient?> FindFirstAsync(
        ISpecification<Patient> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<Patient, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Patient> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Patient, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Patient> AddAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Patients.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Patient>> AddRangeAsync(IEnumerable<Patient> entities, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Update(Patient entity)
    {
        _context.Patients.Update(entity);
    }

    public void UpdateRange(IEnumerable<Patient> entities)
    {
        _context.Patients.UpdateRange(entities);
    }

    public void Delete(Patient entity)
    {
        _context.Patients.Remove(entity);
    }

    public void DeleteRange(IEnumerable<Patient> entities)
    {
        _context.Patients.RemoveRange(entities);
    }

    public async Task<bool> DeleteByIdAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdAsync(id, cancellationToken);
        if (patient == null) return false;
        
        Delete(patient);
        return true;
    }

    public async Task<Patient?> GetByMedicalRecordNumberAsync(
        MedicalRecordNumber mrn,
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.MedicalRecordNumber == mrn, cancellationToken);
    }

    public async Task<IEnumerable<Patient>> GetActivePatientsByAgeRangeAsync(
        int minAge,
        int maxAge,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var maxBirthDate = today.AddYears(-minAge);
        var minBirthDate = today.AddYears(-(maxAge + 1));

        return await _context.Patients
            .Where(p => p.IsActive && 
                       p.DateOfBirth <= maxBirthDate && 
                       p.DateOfBirth > minBirthDate)
            .OrderBy(p => p.Name.LastName)
            .ThenBy(p => p.Name.FirstName)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Patient> ApplySpecification(ISpecification<Patient> specification)
    {
        var query = _context.Patients.AsQueryable();

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