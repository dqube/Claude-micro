using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeAddressRemovedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public AddressId AddressId { get; }

    public EmployeeAddressRemovedEvent(EmployeeId employeeId, AddressId addressId)
    {
        EmployeeId = employeeId;
        AddressId = addressId;
    }
}