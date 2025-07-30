using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeReactivatedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }

    public EmployeeReactivatedEvent(EmployeeId employeeId)
    {
        EmployeeId = employeeId;
    }
}