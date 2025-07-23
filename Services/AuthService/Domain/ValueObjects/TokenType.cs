using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class TokenType : ValueObject
{
    public string Value { get; }

    public TokenType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Token type cannot be null or empty", nameof(value));
        
        if (!IsValidTokenType(value))
            throw new ArgumentException($"Invalid token type: {value}", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TokenType tokenType)
    {
        ArgumentNullException.ThrowIfNull(tokenType);
        return tokenType.Value;
    }
    public static explicit operator TokenType(string value) => new(value);

    private static bool IsValidTokenType(string type)
    {
        return type == "EmailVerification" || type == "PasswordReset";
    }
}
