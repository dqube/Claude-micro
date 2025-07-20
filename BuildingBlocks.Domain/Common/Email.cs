using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Common;

public class Email : SingleValueObject<string>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty", nameof(value));
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format", nameof(value));
    }

    public static implicit operator Email(string email) => new(email);
    public static explicit operator string(Email email) => email.Value;
}