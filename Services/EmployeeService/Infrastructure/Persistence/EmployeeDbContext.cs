using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.Configurations;

namespace EmployeeService.Infrastructure.Persistence;

public class EmployeeDbContext : DbContext, IDbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeContactNumber> EmployeeContactNumbers => Set<EmployeeContactNumber>();
    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();
    
    // Inbox/Outbox pattern tables
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }
    
    // IDbContext implementation
    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeContactNumberConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeAddressConfiguration());
        
        // Apply inbox/outbox configurations - use employee schema
        modelBuilder.ConfigureInboxOutbox("employee");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<Employee>()
            .Ignore(e => e.DomainEvents);
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

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var employeeEntities = ChangeTracker
            .Entries<Employee>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var allDomainEvents = employeeEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        // Clear domain events from entities
        employeeEntities.ForEach(entity => entity.ClearDomainEvents());

        // Save domain events to outbox for eventual consistency
        foreach (var domainEvent in allDomainEvents)
        {
            var eventTypeName = domainEvent.GetType().Name;
            var eventPayload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            
            var outboxMessage = new OutboxMessage(
                messageType: eventTypeName,
                payload: eventPayload,
                destination: "EmployeeService.DomainEvents",
                correlationId: domainEvent.Id.ToString()
            );
            
            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }
}