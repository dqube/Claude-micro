using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeCreatedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public Guid UserId { get; }
    public int StoreId { get; }
    public EmployeeNumber EmployeeNumber { get; }
    public Position Position { get; }
    public DateTime HireDate { get; }

    public EmployeeCreatedEvent(EmployeeId employeeId, Guid userId, int storeId, EmployeeNumber employeeNumber, Position position, DateTime hireDate)
    {
        EmployeeId = employeeId;
        UserId = userId;
        StoreId = storeId;
        EmployeeNumber = employeeNumber;
        Position = position;
        HireDate = hireDate;
    }
}