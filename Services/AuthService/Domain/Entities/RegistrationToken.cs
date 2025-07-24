using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class RegistrationToken : AggregateRoot<TokenId>
{
    public string Token { get; private set; } = string.Empty;
    public Email Email { get; private set; } = new("temp@temp.com");
    public UserId UserId { get; private set; }
    public TokenType TokenType { get; private set; }
    public DateTime Expiration { get; private set; }
    public DateTime ExpiresAt => Expiration;
    public DateTime? UsedAt { get; private set; }
    public UserId? UsedBy { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private RegistrationToken() : base(TokenId.New())
    {
        UserId = UserId.New();
        TokenType = TokenType.EmailVerification;
    }

    public RegistrationToken(
        TokenId id,
        string token,
        Email email,
        UserId userId,
        TokenType tokenType,
        DateTime? expiration = null,
        UserId? createdBy = null) : base(id)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UserId = userId;
        TokenType = tokenType;
        Expiration = expiration ?? DateTime.UtcNow.AddHours(24);
        IsUsed = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        ValidateExpiration();

        AddDomainEvent(new RegistrationTokenCreatedEvent(Id, UserId, TokenType));
    }

    public void MarkAsUsed(UserId? usedBy = null)
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used");

        if (IsExpired())
            throw new InvalidOperationException("Token has expired");

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UsedBy = usedBy;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > Expiration;
    }

    public bool IsValid()
    {
        return !IsUsed && !IsExpired();
    }

    private void ValidateExpiration()
    {
        if (Expiration <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(Expiration));

        // Token should not be valid for more than 7 days
        if (Expiration > DateTime.UtcNow.AddDays(7))
            throw new ArgumentException("Token expiration cannot exceed 7 days", nameof(Expiration));
    }
}