using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeTerminatedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public DateTime TerminationDate { get; }

    public EmployeeTerminatedEvent(EmployeeId employeeId, DateTime terminationDate)
    {
        EmployeeId = employeeId;
        TerminationDate = terminationDate;
    }
}