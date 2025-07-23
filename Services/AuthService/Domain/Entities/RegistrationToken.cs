using BuildingBlocks.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class RegistrationToken : AuditableEntity
{
    public RegistrationTokenId Id { get; private set; }
    public UserId UserId { get; private set; }
    public TokenType TokenType { get; private set; }
    public DateTime Expiration { get; private set; }
    public bool IsUsed { get; private set; }

    public User User { get; private set; }

    // Private constructor for EF Core
    private RegistrationToken() { }

    private RegistrationToken(RegistrationTokenId id, UserId userId, TokenType tokenType, DateTime expiration)
    {
        Id = id;
        UserId = userId;
        TokenType = tokenType;
        Expiration = expiration;
        IsUsed = false;
    }

    public static RegistrationToken Create(UserId userId, TokenType tokenType, DateTime expiration)
    {
        return new RegistrationToken(RegistrationTokenId.New(), userId, tokenType, expiration);
    }

    public void UseToken()
    {
        IsUsed = true;
    }
}
