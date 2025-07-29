using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductBarcodeAddedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public BarcodeId BarcodeId { get; }
    public BarcodeValue BarcodeValue { get; }
    public BarcodeType BarcodeType { get; }

    public ProductBarcodeAddedEvent(ProductId productId, BarcodeId barcodeId, BarcodeValue barcodeValue, BarcodeType barcodeType)
    {
        ProductId = productId;
        BarcodeId = barcodeId;
        BarcodeValue = barcodeValue;
        BarcodeType = barcodeType;
    }
}