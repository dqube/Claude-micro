using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Application.Services;

namespace BuildingBlocks.Infrastructure.Data.Interceptors;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventService _domainEventService;

    public DomainEventInterceptor(IDomainEventService domainEventService)
    {
        ArgumentNullException.ThrowIfNull(domainEventService);
        _domainEventService = domainEventService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null) return;

        var aggregateRoots = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity.GetType().BaseType?.IsGenericType == true && 
                       e.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(AggregateRoot<>))
            .Select(e => e.Entity)
            .Cast<dynamic>()
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(ar => (IEnumerable<IDomainEvent>)ar.DomainEvents)
            .ToList();

        aggregateRoots.ForEach(ar => ar.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _domainEventService.PublishAsync(domainEvent);
        }
    }
}