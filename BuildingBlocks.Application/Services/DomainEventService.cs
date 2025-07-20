using BuildingBlocks.Domain.DomainEvents;

namespace BuildingBlocks.Application.Services;

public class DomainEventService : IDomainEventService
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventService(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(domainEvent, cancellationToken);
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}