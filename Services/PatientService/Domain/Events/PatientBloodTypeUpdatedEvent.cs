using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientBloodTypeUpdatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }
    public BloodType BloodType { get; }

    public PatientBloodTypeUpdatedEvent(PatientId patientId, BloodType bloodType)
    {
        PatientId = patientId;
        BloodType = bloodType;
    }
}