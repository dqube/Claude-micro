using BuildingBlocks.Domain.ValueObjects;

namespace SharedService.Domain.ValueObjects;

public class CurrencyName : ValueObject
{
    public string Value { get; }

    public CurrencyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Currency name cannot be empty", nameof(value));
        
        if (value.Length > 50)
            throw new ArgumentException("Currency name cannot exceed 50 characters", nameof(value));

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(CurrencyName currencyName) => currencyName.Value;
    public static implicit operator CurrencyName(string value) => new(value);
} 