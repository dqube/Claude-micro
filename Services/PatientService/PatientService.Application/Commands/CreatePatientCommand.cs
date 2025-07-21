using BuildingBlocks.Application.CQRS.Commands;
using PatientService.Application.DTOs;

namespace PatientService.Application.Commands;

public class CreatePatientCommand : CommandBase<PatientDto>
{
    public string MedicalRecordNumber { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? MiddleName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public AddressDto? Address { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; }
    public string? BloodType { get; init; }

    public CreatePatientCommand(
        string medicalRecordNumber,
        string firstName,
        string lastName,
        string? middleName,
        string email,
        string? phoneNumber,
        AddressDto? address,
        DateTime dateOfBirth,
        string gender,
        string? bloodType = null)
    {
        MedicalRecordNumber = medicalRecordNumber;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        BloodType = bloodType;
    }
}