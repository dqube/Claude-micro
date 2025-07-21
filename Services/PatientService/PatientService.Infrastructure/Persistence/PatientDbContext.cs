using BuildingBlocks.Domain.DomainEvents;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Configurations;

namespace PatientService.Infrastructure.Persistence;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PatientConfiguration());

        // Configure strongly typed IDs
        modelBuilder.Entity<Patient>()
            .Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PatientId.From(value));

        // Ignore domain events for EF Core
        modelBuilder.Entity<Patient>()
            .Ignore(p => p.DomainEvents);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events before saving changes
        await PublishDomainEventsAsync(cancellationToken);
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<Patient>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // Here you would publish the domain event using your preferred method
            // For example, using MediatR or a service bus
            // await _mediator.Publish(domainEvent, cancellationToken);
            
            // For now, we'll just clear the events
            await Task.CompletedTask;
        }
    }
}