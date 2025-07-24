using BuildingBlocks.Domain.Entities;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class RegistrationToken : AggregateRoot<TokenId>
{
    public UserId UserId { get; private set; }
    public TokenType TokenType { get; private set; }
    public DateTime Expiration { get; private set; }
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
        UserId userId,
        TokenType tokenType,
        DateTime? expiration = null,
        UserId? createdBy = null) : base(id)
    {
        UserId = userId;
        TokenType = tokenType;
        Expiration = expiration ?? DateTime.UtcNow.AddHours(24);
        IsUsed = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        ValidateExpiration();

        AddDomainEvent(new RegistrationTokenCreatedEvent(Id, UserId, TokenType));
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used");

        if (IsExpired())
            throw new InvalidOperationException("Token has expired");

        IsUsed = true;
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