using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductBarcodeRemovedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public BarcodeId BarcodeId { get; }
    public BarcodeValue BarcodeValue { get; }

    public ProductBarcodeRemovedEvent(ProductId productId, BarcodeId barcodeId, BarcodeValue barcodeValue)
    {
        ProductId = productId;
        BarcodeId = barcodeId;
        BarcodeValue = barcodeValue;
    }
}