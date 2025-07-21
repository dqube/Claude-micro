using BuildingBlocks.Application.CQRS.Commands;
using PatientService.Application.DTOs;

namespace PatientService.Application.Commands;

public class CreatePatientCommand : CommandBase<PatientDto>
{
    public string MedicalRecordNumber { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }
    public string Email { get; }
    public string? PhoneNumber { get; }
    public AddressDto? Address { get; }
    public DateTime DateOfBirth { get; }
    public string Gender { get; }
    public string? BloodType { get; }

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