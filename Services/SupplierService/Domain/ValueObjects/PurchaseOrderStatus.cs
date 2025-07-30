using BuildingBlocks.Domain.ValueObjects;

namespace SupplierService.Domain.ValueObjects;

public class PurchaseOrderStatus : ValueObject
{
    public string Value { get; }

    private PurchaseOrderStatus(string value)
    {
        Value = value;
    }

    public static readonly PurchaseOrderStatus Draft = new("Draft");
    public static readonly PurchaseOrderStatus Ordered = new("Ordered");
    public static readonly PurchaseOrderStatus Received = new("Received");
    public static readonly PurchaseOrderStatus Cancelled = new("Cancelled");

    public static PurchaseOrderStatus From(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "draft" => Draft,
            "ordered" => Ordered,
            "received" => Received,
            "cancelled" => Cancelled,
            _ => throw new ArgumentException($"Invalid purchase order status: {value}", nameof(value))
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(PurchaseOrderStatus status) => status.Value;
} 