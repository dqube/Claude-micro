using BuildingBlocks.Domain.ValueObjects;

namespace SharedService.Domain.ValueObjects;

public class CountryName : ValueObject
{
    public string Value { get; }

    public CountryName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country name cannot be empty", nameof(value));
        
        if (value.Length > 100)
            throw new ArgumentException("Country name cannot exceed 100 characters", nameof(value));

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(CountryName countryName) => countryName.Value;
    public static implicit operator CountryName(string value) => new(value);
} 