using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Configurations;

namespace InventoryService.Infrastructure.Persistence;

public class InventoryDbContext : DbContext, IDbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("inventory");

        modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
        modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
    }

}