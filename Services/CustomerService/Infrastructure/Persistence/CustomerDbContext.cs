using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Configurations;

namespace CustomerService.Infrastructure.Persistence;

public class CustomerDbContext : DbContext, IDbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerContactNumber> CustomerContactNumbers => Set<CustomerContactNumber>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    
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
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerContactNumberConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerAddressConfiguration());
        
        // Apply inbox/outbox configurations - use customer schema
        modelBuilder.ConfigureInboxOutbox("customer");

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();
    }
} 