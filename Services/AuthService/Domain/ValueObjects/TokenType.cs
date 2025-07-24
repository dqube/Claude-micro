using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class TokenType : SingleValueObject<string>
{
    public static readonly TokenType EmailVerification = new("EmailVerification");
    public static readonly TokenType PasswordReset = new("PasswordReset");

    public TokenType(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Token type cannot be null or empty", nameof(value));
        
        if (!IsValidTokenType(value))
            throw new ArgumentException($"Invalid token type: {value}", nameof(value));
    }
    
    private static bool IsValidTokenType(string value)
    {
        return value is "EmailVerification" or "PasswordReset";
    }
}