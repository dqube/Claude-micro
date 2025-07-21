using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientDeactivatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }

    public PatientDeactivatedEvent(PatientId patientId)
    {
        PatientId = patientId;
    }
}