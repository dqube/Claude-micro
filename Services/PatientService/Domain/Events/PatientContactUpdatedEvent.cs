using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientContactUpdatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }
    public Email Email { get; }
    public PhoneNumber? PhoneNumber { get; }

    public PatientContactUpdatedEvent(PatientId patientId, Email email, PhoneNumber? phoneNumber)
    {
        PatientId = patientId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}