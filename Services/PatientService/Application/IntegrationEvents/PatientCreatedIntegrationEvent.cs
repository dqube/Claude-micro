using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;

namespace PatientService.Application.IntegrationEvents;

/// <summary>
/// Integration event published when a patient is created - used for cross-service communication
/// </summary>
public class PatientCreatedIntegrationEvent : IntegrationEventBase, IMessage
{
    public Guid PatientId { get; init; }
    public string MedicalRecordNumber { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime CreatedAt { get; init; }

    // IMessage implementation
    Guid IMessage.Id => Id;
    DateTime IMessage.Timestamp => OccurredOn;

    public PatientCreatedIntegrationEvent(
        Guid patientId,
        string medicalRecordNumber,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string gender,
        string email,
        string? phoneNumber,
        DateTime createdAt)
        : base("PatientService", Guid.NewGuid().ToString())
    {
        PatientId = patientId;
        MedicalRecordNumber = medicalRecordNumber;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAt = createdAt;
    }
}
