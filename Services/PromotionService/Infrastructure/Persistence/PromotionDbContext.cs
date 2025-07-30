using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PromotionService.Domain.Entities;
using PromotionService.Infrastructure.Configurations;

namespace PromotionService.Infrastructure.Persistence;

public class PromotionDbContext : DbContext, IDbContext
{
    public PromotionDbContext(DbContextOptions<PromotionDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<DiscountType> DiscountTypes => Set<DiscountType>();
    public DbSet<DiscountCampaign> DiscountCampaigns => Set<DiscountCampaign>();
    public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<PromotionProduct> PromotionProducts => Set<PromotionProduct>();
    
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
        modelBuilder.ApplyConfiguration(new DiscountTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountCampaignConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountRuleConfiguration());
        modelBuilder.ApplyConfiguration(new PromotionConfiguration());
        modelBuilder.ApplyConfiguration(new PromotionProductConfiguration());
        
        // Apply inbox/outbox configurations - use promotion schema
        modelBuilder.ConfigureInboxOutbox("promotion");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<DiscountCampaign>()
            .Ignore(dc => dc.DomainEvents);
        
        modelBuilder.Entity<DiscountRule>()
            .Ignore(dr => dr.DomainEvents);
            
        modelBuilder.Entity<Promotion>()
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

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var campaignEntities = ChangeTracker
            .Entries<DiscountCampaign>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
            
        var ruleEntities = ChangeTracker
            .Entries<DiscountRule>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
            
        var promotionEntities = ChangeTracker
            .Entries<Promotion>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var allDomainEvents = campaignEntities
            .SelectMany(x => x.DomainEvents)
            .Concat(ruleEntities.SelectMany(x => x.DomainEvents))
            .Concat(promotionEntities.SelectMany(x => x.DomainEvents))
            .ToList();

        // Clear domain events from entities
        campaignEntities.ForEach(entity => entity.ClearDomainEvents());
        ruleEntities.ForEach(entity => entity.ClearDomainEvents());
        promotionEntities.ForEach(entity => entity.ClearDomainEvents());

        // Save domain events to outbox for eventual consistency
        foreach (var domainEvent in allDomainEvents)
        {
            var eventTypeName = domainEvent.GetType().Name;
            var eventPayload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            
            var outboxMessage = new OutboxMessage(
                messageType: eventTypeName,
                payload: eventPayload,
                destination: "PromotionService.DomainEvents",
                correlationId: domainEvent.Id.ToString()
            );
            
            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }
} 