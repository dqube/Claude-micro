using BuildingBlocks.Application.DTOs;

namespace PatientService.Application.DTOs;

public class PatientDto : BaseDto
{
    public Guid Id { get; set; }
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public AddressDto? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodType { get; set; }
    public bool IsActive { get; set; }
    public bool IsMinor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}