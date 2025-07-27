using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Configurations;

namespace PatientService.Infrastructure.Persistence;

public class PatientDbContext : DbContext, IDbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<Patient> Patients => Set<Patient>();
    
    // Inbox/Outbox pattern tables
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    // IDbContext implementation
    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        
        // Apply inbox/outbox configurations - use patients schema
        modelBuilder.ConfigureInboxOutbox("patients");

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

        // Save domain events to outbox for eventual consistency
        foreach (var domainEvent in domainEvents)
        {
            var eventTypeName = domainEvent.GetType().Name;
            var eventPayload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            
            var outboxMessage = new OutboxMessage(
                messageType: eventTypeName,
                payload: eventPayload,
                destination: "PatientService.DomainEvents",
                correlationId: domainEvent.Id.ToString()
            );
            
            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }
}