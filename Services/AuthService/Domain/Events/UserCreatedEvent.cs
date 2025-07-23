using BuildingBlocks.Domain.Events;

namespace AuthService.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Username, string Email) : DomainEvent;
