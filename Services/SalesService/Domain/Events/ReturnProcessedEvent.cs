using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class ReturnProcessedEvent : DomainEventBase
{
    public ReturnId ReturnId { get; }
    public SaleId SaleId { get; }
    public decimal TotalRefund { get; }

    public ReturnProcessedEvent(ReturnId returnId, SaleId saleId, decimal totalRefund)
    {
        ReturnId = returnId;
        SaleId = saleId;
        TotalRefund = totalRefund;
    }
}