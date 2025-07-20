using BuildingBlocks.Domain.DomainEvents;

namespace BuildingBlocks.Application.Services;

public interface IDomainEventService
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}