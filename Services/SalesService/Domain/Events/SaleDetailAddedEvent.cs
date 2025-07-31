using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class SaleDetailAddedEvent : DomainEventBase
{
    public SaleId SaleId { get; }
    public SaleDetailId SaleDetailId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public SaleDetailAddedEvent(SaleId saleId, SaleDetailId saleDetailId, Guid productId, int quantity, decimal unitPrice)
    {
        SaleId = saleId;
        SaleDetailId = saleDetailId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}