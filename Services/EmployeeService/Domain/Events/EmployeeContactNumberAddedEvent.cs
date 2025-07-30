using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeContactNumberAddedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public ContactNumberId ContactNumberId { get; }
    public string PhoneNumber { get; }

    public EmployeeContactNumberAddedEvent(EmployeeId employeeId, ContactNumberId contactNumberId, string phoneNumber)
    {
        EmployeeId = employeeId;
        ContactNumberId = contactNumberId;
        PhoneNumber = phoneNumber;
    }
}