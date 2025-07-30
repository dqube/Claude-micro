using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeePositionUpdatedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public Position OldPosition { get; }
    public Position NewPosition { get; }

    public EmployeePositionUpdatedEvent(EmployeeId employeeId, Position oldPosition, Position newPosition)
    {
        EmployeeId = employeeId;
        OldPosition = oldPosition;
        NewPosition = newPosition;
    }
}