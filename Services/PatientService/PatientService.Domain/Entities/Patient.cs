using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using PatientService.Domain.Events;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Entities;

public class Patient : AggregateRoot<PatientId>
{
    public MedicalRecordNumber MedicalRecordNumber { get; private set; }
    public PatientName Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Address? Address { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public BloodType? BloodType { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private Patient() : base(PatientId.New()) 
    { 
        MedicalRecordNumber = new MedicalRecordNumber("TEMP-00000");
        Name = new PatientName("Unknown", "Unknown");
        Email = new Email("unknown@temp.com");
        Gender = Gender.PreferNotToSay;
    }

    public Patient(
        PatientId id,
        MedicalRecordNumber medicalRecordNumber,
        PatientName name,
        Email email,
        DateTime dateOfBirth,
        Gender gender) : base(id)
    {
        ValidateDateOfBirth(dateOfBirth);

        MedicalRecordNumber = medicalRecordNumber;
        Name = name;
        Email = email;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientCreatedEvent(Id, MedicalRecordNumber, Name));
    }

    public void UpdateContactInformation(Email email, PhoneNumber? phoneNumber = null)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientContactUpdatedEvent(Id, Email, PhoneNumber));
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientAddressUpdatedEvent(Id, Address));
    }

    public void UpdateBloodType(BloodType bloodType)
    {
        BloodType = bloodType;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientBloodTypeUpdatedEvent(Id, BloodType));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PatientActivatedEvent(Id));
    }

    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }

    public bool IsMinor() => CalculateAge() < 18;

    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future", nameof(dateOfBirth));

        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
            age--;

        if (age > 150)
            throw new ArgumentException("Patient cannot be older than 150 years", nameof(dateOfBirth));
    }
}