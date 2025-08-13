using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Configurations;

namespace PaymentService.Infrastructure.Persistence;

public class PaymentDbContext : DbContext, IDbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<PaymentProcessor> PaymentProcessors => Set<PaymentProcessor>();
    
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

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new PaymentProcessorConfiguration());
        
        // Apply inbox/outbox configurations - use payment schema
        modelBuilder.ConfigureInboxOutbox("payment");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<PaymentProcessor>()
            .Ignore(p => p.DomainEvents);
    }
} 