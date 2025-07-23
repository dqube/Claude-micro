using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.DomainEvents;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Events;

public class PatientAddressUpdatedEvent : DomainEventBase
{
    public PatientId PatientId { get; }
    public Address Address { get; }

    public PatientAddressUpdatedEvent(PatientId patientId, Address address)
    {
        PatientId = patientId;
        Address = address;
    }
}