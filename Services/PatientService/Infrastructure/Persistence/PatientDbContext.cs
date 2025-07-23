using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Configurations;

namespace PatientService.Infrastructure.Persistence;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PatientConfiguration());

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<Patient>()
            .Ignore(p => p.DomainEvents);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            // This will be overridden by the options passed in the constructor
            // but we include it here for completeness
        }

        // Enable strongly typed ID value converters
        optionsBuilder.UseStronglyTypedIdConverters();
        
        base.OnConfiguring(optionsBuilder);
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
            await Task.CompletedTask;
        }
    }
}