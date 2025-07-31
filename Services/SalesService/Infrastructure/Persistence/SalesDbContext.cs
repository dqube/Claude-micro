using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SalesService.Domain.Entities;
using SalesService.Infrastructure.Configurations;

namespace SalesService.Infrastructure.Persistence;

public class SalesDbContext : DbContext, IDbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
    public DbSet<AppliedDiscount> AppliedDiscounts => Set<AppliedDiscount>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<ReturnDetail> ReturnDetails => Set<ReturnDetail>();
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleDetailConfiguration());
        modelBuilder.ApplyConfiguration(new AppliedDiscountConfiguration());
        modelBuilder.ApplyConfiguration(new ReturnConfiguration());
        modelBuilder.ApplyConfiguration(new ReturnDetailConfiguration());
        
        modelBuilder.ConfigureInboxOutbox("sales");
        modelBuilder.ConfigureStronglyTypedIds();

        modelBuilder.Entity<Sale>()
            .Ignore(s => s.DomainEvents);
        
        modelBuilder.Entity<Return>()
            .Ignore(r => r.DomainEvents);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        optionsBuilder.UseStronglyTypedIdConverters();
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var saleEntities = ChangeTracker
            .Entries<Sale>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
            
        var returnEntities = ChangeTracker
            .Entries<Return>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var allDomainEvents = saleEntities
            .SelectMany(x => x.DomainEvents)
            .Concat(returnEntities.SelectMany(x => x.DomainEvents))
            .ToList();

        saleEntities.ForEach(entity => entity.ClearDomainEvents());
        returnEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in allDomainEvents)
        {
            var eventTypeName = domainEvent.GetType().Name;
            var eventPayload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            
            var outboxMessage = new OutboxMessage(
                messageType: eventTypeName,
                payload: eventPayload,
                destination: "SalesService.DomainEvents",
                correlationId: domainEvent.Id.ToString()
            );
            
            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }
}