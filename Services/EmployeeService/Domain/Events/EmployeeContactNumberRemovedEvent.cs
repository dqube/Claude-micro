using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeContactNumberRemovedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public ContactNumberId ContactNumberId { get; }

    public EmployeeContactNumberRemovedEvent(EmployeeId employeeId, ContactNumberId contactNumberId)
    {
        EmployeeId = employeeId;
        ContactNumberId = contactNumberId;
    }
}