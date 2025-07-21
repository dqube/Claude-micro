using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using PatientService.Application.DTOs;
using PatientService.Domain.Entities;
using PatientService.Domain.Exceptions;
using PatientService.Domain.ValueObjects;

namespace PatientService.Application.Queries;

public class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientDto>
{
    private readonly IReadOnlyRepository<Patient, PatientId> _patientRepository;

    public GetPatientByIdQueryHandler(IReadOnlyRepository<Patient, PatientId> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto> HandleAsync(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patientId = PatientId.From(request.PatientId);
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        
        if (patient == null)
            throw new PatientNotFoundException(patientId);

        return MapToDto(patient);
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
            Address = patient.Address != null ? new AddressDto
            {
                Street = patient.Address.Street,
                City = patient.Address.City,
                PostalCode = patient.Address.PostalCode,
                Country = patient.Address.Country
            } : null,
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