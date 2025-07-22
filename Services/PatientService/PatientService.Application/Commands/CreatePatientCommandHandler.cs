using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using PatientService.Application.DTOs;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;

namespace PatientService.Application.Commands;

public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, PatientDto>
{
    private readonly IRepository<Patient, PatientId> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(
        IRepository<Patient, PatientId> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PatientDto> HandleAsync(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "CreatePatientCommand cannot be null");
        }
        var patientId = PatientId.New();
        var mrn = new MedicalRecordNumber(request.MedicalRecordNumber);
        var name = new PatientName(request.FirstName, request.LastName, request.MiddleName);
        var email = new Email(request.Email);
        var gender = Gender.FromName(request.Gender);

        var patient = new Patient(patientId, mrn, name, email, request.DateOfBirth, gender);

        // Add optional fields
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var phoneNumber = new PhoneNumber(request.PhoneNumber);
            patient.UpdateContactInformation(email, phoneNumber);
        }

        if (request.Address != null)
        {
            var address = new Address(
                request.Address.Street,
                request.Address.City,
                request.Address.PostalCode,
                request.Address.Country);
            patient.UpdateAddress(address);
        }

        if (!string.IsNullOrEmpty(request.BloodType))
        {
            var bloodType = BloodType.FromName(request.BloodType);
            patient.UpdateBloodType(bloodType);
        }

        await _patientRepository.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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