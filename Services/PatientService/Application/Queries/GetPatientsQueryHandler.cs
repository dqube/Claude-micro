using System.Globalization;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Specifications;
using PatientService.Application.DTOs;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;

namespace PatientService.Application.Queries;

public class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, PagedResult<PatientDto>>
{
    private readonly IReadOnlyRepository<Patient, PatientId> _patientRepository;

    public GetPatientsQueryHandler(IReadOnlyRepository<Patient, PatientId> patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task<PagedResult<PatientDto>> HandleAsync(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var specification = new PatientFilterSpecification(request);
        
        var patients = await _patientRepository.FindAsync(specification, cancellationToken);
        var totalCount = await _patientRepository.CountAsync(specification, cancellationToken);

        // Apply pagination manually since the repository doesn't have GetPagedAsync
        var pagedPatients = patients
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        var patientDtos = pagedPatients.Select(MapToDto).ToList();

        return new PagedResult<PatientDto>(patientDtos, totalCount, request.Page, request.PageSize);
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id.Value,
            MedicalRecordNumber = patient.MedicalRecordNumber.Value,
            FirstName = patient.Name.FirstName,
            LastName = patient.Name.LastName,
            MiddleName = patient.Name.MiddleName,
            FullName = patient.Name.FullName,
            DisplayName = patient.Name.DisplayName,
            Email = patient.Email.Value,
            PhoneNumber = patient.PhoneNumber?.Value,
            Address = patient.Address is not null 
                ? new AddressDto(
                    patient.Address.Street,
                    patient.Address.City,
                    patient.Address.PostalCode,
                    patient.Address.Country
                ) : null,
            DateOfBirth = patient.DateOfBirth,
            Age = patient.CalculateAge(),
            Gender = patient.Gender.Name,
            BloodType = patient.BloodType?.Name,
            IsActive = patient.IsActive,
            IsMinor = patient.IsMinor(),
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        };
    }
}

public class PatientFilterSpecification : Specification<Patient>
{
    public override System.Linq.Expressions.Expression<Func<Patient, bool>> Criteria { get; }

    public PatientFilterSpecification(GetPatientsQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var criteria = PredicateBuilder.True<Patient>();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            criteria = criteria.And(p => 
                p.Name.FirstName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Name.LastName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.MedicalRecordNumber.Value.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (query.IsActive.HasValue)
        {
            criteria = criteria.And(p => p.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(query.Gender))
        {
            criteria = criteria.And(p => p.Gender.Name == query.Gender);
        }

        if (query.MinAge.HasValue)
        {
            var maxBirthDate = DateTime.Today.AddYears(-query.MinAge.Value);
            criteria = criteria.And(p => p.DateOfBirth <= maxBirthDate);
        }

        if (query.MaxAge.HasValue)
        {
            var minBirthDate = DateTime.Today.AddYears(-(query.MaxAge.Value + 1));
            criteria = criteria.And(p => p.DateOfBirth > minBirthDate);
        }

        Criteria = criteria;

        ApplyOrderBy(p => p.Name.LastName);
        ApplyOrderBy(p => p.Name.FirstName);
    }
}

public static class PredicateBuilder
{
    public static System.Linq.Expressions.Expression<Func<T, bool>> True<T>() => f => true;
    public static System.Linq.Expressions.Expression<Func<T, bool>> False<T>() => f => false;

    public static System.Linq.Expressions.Expression<Func<T, bool>> And<T>(
        this System.Linq.Expressions.Expression<Func<T, bool>> expr1,
        System.Linq.Expressions.Expression<Func<T, bool>> expr2)
    {
        ArgumentNullException.ThrowIfNull(expr1);
        ArgumentNullException.ThrowIfNull(expr2);
        
        var invokedExpr = System.Linq.Expressions.Expression.Invoke(expr2, expr1.Parameters.Cast<System.Linq.Expressions.Expression>());
        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
            System.Linq.Expressions.Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}