using BuildingBlocks.Domain.ValueObjects;

namespace SharedService.Domain.ValueObjects;

public class CurrencySymbol : ValueObject
{
    public string Value { get; }

    public CurrencySymbol(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Currency symbol cannot be empty", nameof(value));
        
        if (value.Length > 5)
            throw new ArgumentException("Currency symbol cannot exceed 5 characters", nameof(value));

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(CurrencySymbol currencySymbol) => currencySymbol.Value;
    public static implicit operator CurrencySymbol(string value) => new(value);
} 