using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientActivatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }

    public PatientActivatedEvent(PatientId patientId)
    {
        PatientId = patientId;
    }
}