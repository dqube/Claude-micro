using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ReportingService.Domain.Entities;
using ReportingService.Infrastructure.Configurations;

namespace ReportingService.Infrastructure.Persistence;

public class ReportingDbContext : DbContext, IDbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<SalesSnapshot> SalesSnapshots => Set<SalesSnapshot>();
    public DbSet<InventorySnapshot> InventorySnapshots => Set<InventorySnapshot>();
    public DbSet<PromotionEffectiveness> PromotionEffectiveness => Set<PromotionEffectiveness>();
    
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
        modelBuilder.ApplyConfiguration(new SalesSnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new InventorySnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new PromotionEffectivenessConfiguration());
        
        // Apply inbox/outbox configurations - use reporting schema
        modelBuilder.ConfigureInboxOutbox("reporting");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<SalesSnapshot>()
            .Ignore(s => s.DomainEvents);
            
        modelBuilder.Entity<InventorySnapshot>()
            .Ignore(i => i.DomainEvents);
            
        modelBuilder.Entity<PromotionEffectiveness>()
            .Ignore(p => p.DomainEvents);
    }
} 