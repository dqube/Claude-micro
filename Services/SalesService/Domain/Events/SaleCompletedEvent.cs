using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class SaleCompletedEvent : DomainEventBase
{
    public SaleId SaleId { get; }
    public decimal TotalAmount { get; }
    public DateTime TransactionTime { get; }

    public SaleCompletedEvent(SaleId saleId, decimal totalAmount, DateTime transactionTime)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
        TransactionTime = transactionTime;
    }
}