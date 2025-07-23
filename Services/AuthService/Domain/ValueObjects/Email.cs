using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace AuthService.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty", nameof(value));
        
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = value.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        return email.Value;
    }
    public static explicit operator Email(string value) => new(value);

    private static bool IsValidEmail(string email)
    {
        // Basic regex for email validation
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
