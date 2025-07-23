using BuildingBlocks.Application.DTOs;

namespace PatientService.Application.DTOs;

public record PatientDto
{
    public Guid Id { get; init; }
    public string MedicalRecordNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public AddressDto? Address { get; init; }
    public DateTime DateOfBirth { get; init; }
    public int Age { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? BloodType { get; init; }
    public bool IsActive { get; init; }
    public bool IsMinor { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country
);