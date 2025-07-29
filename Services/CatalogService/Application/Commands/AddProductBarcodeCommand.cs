using BuildingBlocks.Application.CQRS.Commands;

namespace CatalogService.Application.Commands;

public class AddProductBarcodeCommand : CommandBase
{
    public Guid ProductId { get; init; }
    public string BarcodeValue { get; init; } = string.Empty;
    public string BarcodeType { get; init; } = string.Empty;

    public AddProductBarcodeCommand(Guid productId, string barcodeValue, string barcodeType)
    {
        ProductId = productId;
        BarcodeValue = barcodeValue;
        BarcodeType = barcodeType;
    }
}