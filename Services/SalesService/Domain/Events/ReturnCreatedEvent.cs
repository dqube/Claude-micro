using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class ReturnCreatedEvent : DomainEventBase
{
    public ReturnId ReturnId { get; }
    public SaleId SaleId { get; }
    public Guid EmployeeId { get; }
    public Guid? CustomerId { get; }

    public ReturnCreatedEvent(ReturnId returnId, SaleId saleId, Guid employeeId, Guid? customerId)
    {
        ReturnId = returnId;
        SaleId = saleId;
        EmployeeId = employeeId;
        CustomerId = customerId;
    }
}