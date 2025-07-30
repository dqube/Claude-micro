using BuildingBlocks.Domain.StronglyTypedIds;

namespace SharedService.Domain.ValueObjects;

public class CountryCode : StronglyTypedId<string>
{
    public CountryCode(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CountryCode cannot be empty", nameof(value));
        
        if (value.Length != 2)
            throw new ArgumentException("CountryCode must be exactly 2 characters", nameof(value));
        
        if (!value.All(char.IsLetter))
            throw new ArgumentException("CountryCode must contain only letters", nameof(value));
    }
    
    public static CountryCode From(string value) => new(value.ToUpperInvariant());
} 