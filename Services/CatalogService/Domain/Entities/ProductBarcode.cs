using BuildingBlocks.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Entities;

public class ProductBarcode : Entity<BarcodeId>
{
    public ProductId ProductId { get; private set; }
    public BarcodeValue BarcodeValue { get; private set; }
    public BarcodeType BarcodeType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    private ProductBarcode() : base(BarcodeId.New())
    {
        ProductId = ProductId.New();
        BarcodeValue = BarcodeValue.From("TEMP");
        BarcodeType = BarcodeType.Default;
    }

    public ProductBarcode(
        BarcodeId id,
        ProductId productId,
        BarcodeValue barcodeValue,
        BarcodeType barcodeType,
        Guid? createdBy = null) : base(id)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        BarcodeValue = barcodeValue ?? throw new ArgumentNullException(nameof(barcodeValue));
        BarcodeType = barcodeType ?? throw new ArgumentNullException(nameof(barcodeType));
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}