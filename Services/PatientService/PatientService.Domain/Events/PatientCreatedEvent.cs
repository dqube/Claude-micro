using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientCreatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }
    public MedicalRecordNumber MedicalRecordNumber { get; }
    public PatientName Name { get; }

    public PatientCreatedEvent(PatientId patientId, MedicalRecordNumber medicalRecordNumber, PatientName name)
    {
        PatientId = patientId;
        MedicalRecordNumber = medicalRecordNumber;
        Name = name;
    }
}