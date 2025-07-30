using BuildingBlocks.Domain.DomainEvents;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Events;

public class EmployeeAddressAddedEvent : DomainEventBase
{
    public EmployeeId EmployeeId { get; }
    public AddressId AddressId { get; }
    public int AddressTypeId { get; }

    public EmployeeAddressAddedEvent(EmployeeId employeeId, AddressId addressId, int addressTypeId)
    {
        EmployeeId = employeeId;
        AddressId = addressId;
        AddressTypeId = addressTypeId;
    }
}