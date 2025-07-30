using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using SupplierService.Domain.Entities;
using SupplierService.Infrastructure.Configurations;

namespace SupplierService.Infrastructure.Persistence;

public class SupplierDbContext : DbContext, IDbContext
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierContact> SupplierContacts => Set<SupplierContact>();
    public DbSet<SupplierContactNumber> SupplierContactNumbers => Set<SupplierContactNumber>();
    public DbSet<SupplierAddress> SupplierAddresses => Set<SupplierAddress>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();

    public SupplierDbContext(DbContextOptions<SupplierDbContext> options) : base(options)
    {
    }

    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierContactConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierContactNumberConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierAddressConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderDetailConfiguration());
    }
} 