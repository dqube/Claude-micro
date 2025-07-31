using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class SaleCreatedEvent : DomainEventBase
{
    public SaleId SaleId { get; }
    public int StoreId { get; }
    public Guid EmployeeId { get; }
    public Guid? CustomerId { get; }

    public SaleCreatedEvent(SaleId saleId, int storeId, Guid employeeId, Guid? customerId)
    {
        SaleId = saleId;
        StoreId = storeId;
        EmployeeId = employeeId;
        CustomerId = customerId;
    }
}