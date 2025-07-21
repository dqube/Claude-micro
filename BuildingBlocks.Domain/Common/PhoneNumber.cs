using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Common;

public class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneNumberRegex = new(@"^[\+]?[0-9\-\(\)\s]{7,20}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(value));

        var cleanedValue = value.Trim();
        
        if (!PhoneNumberRegex.IsMatch(cleanedValue))
            throw new ArgumentException("Invalid phone number format", nameof(value));

        Value = cleanedValue;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    public static explicit operator PhoneNumber(string value) => new(value);
}