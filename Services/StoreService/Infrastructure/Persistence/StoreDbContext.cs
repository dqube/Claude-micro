using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;
using StoreService.Infrastructure.Configurations;

namespace StoreService.Infrastructure.Persistence;

public class StoreDbContext : DbContext, IDbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Register> Registers => Set<Register>();
    
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

        modelBuilder.ApplyConfiguration(new StoreConfiguration());
        modelBuilder.ApplyConfiguration(new RegisterConfiguration());
        
        // Apply inbox/outbox configurations - use store schema
        modelBuilder.ConfigureInboxOutbox("store");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<Store>()
            .Ignore(s => s.DomainEvents);
        
        modelBuilder.Entity<Register>()
            .Ignore(r => r.DomainEvents);
    }
} 