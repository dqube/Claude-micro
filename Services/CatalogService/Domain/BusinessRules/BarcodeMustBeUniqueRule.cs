using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class BarcodeMustBeUniqueRule : BusinessRuleBase
{
    private readonly BarcodeValue _barcodeValue;
    private readonly IEnumerable<BarcodeValue> _existingBarcodes;

    public BarcodeMustBeUniqueRule(BarcodeValue barcodeValue, IEnumerable<BarcodeValue> existingBarcodes)
    {
        _barcodeValue = barcodeValue;
        _existingBarcodes = existingBarcodes ?? Enumerable.Empty<BarcodeValue>();
    }

    public override bool IsBroken()
    {
        return _existingBarcodes.Any(b => b.Value.Equals(_barcodeValue.Value, StringComparison.OrdinalIgnoreCase));
    }

    public override string Message => $"Barcode '{_barcodeValue.Value}' is already in use";
}