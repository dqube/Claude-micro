using BuildingBlocks.Domain.StronglyTypedIds;

namespace SharedService.Domain.ValueObjects;

public class CurrencyCode : StronglyTypedId<string>
{
    public CurrencyCode(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CurrencyCode cannot be empty", nameof(value));
        
        if (value.Length != 3)
            throw new ArgumentException("CurrencyCode must be exactly 3 characters", nameof(value));
        
        if (!value.All(char.IsLetter))
            throw new ArgumentException("CurrencyCode must contain only letters", nameof(value));
    }
    
    public static CurrencyCode From(string value) => new(value.ToUpperInvariant());
} 