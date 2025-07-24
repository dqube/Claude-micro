using BuildingBlocks.Domain.DomainEvents;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public class RegistrationTokenCreatedEvent : DomainEventBase
{
    public TokenId TokenId { get; }
    public UserId UserId { get; }
    public TokenType TokenType { get; }

    public RegistrationTokenCreatedEvent(TokenId tokenId, UserId userId, TokenType tokenType)
    {
        TokenId = tokenId;
        UserId = userId;
        TokenType = tokenType;
    }
}