using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeAuthLevelUpdatedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public int OldAuthLevel { get; }
    public int NewAuthLevel { get; }

    public EmployeeAuthLevelUpdatedEvent(EmployeeId employeeId, int oldAuthLevel, int newAuthLevel)
    {
        EmployeeId = employeeId;
        OldAuthLevel = oldAuthLevel;
        NewAuthLevel = newAuthLevel;
    }
}