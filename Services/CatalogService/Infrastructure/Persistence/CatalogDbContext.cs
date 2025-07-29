using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Configurations;

namespace CatalogService.Infrastructure.Persistence;

public class CatalogDbContext : DbContextBase
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductBarcode> ProductBarcodes { get; set; }
    public DbSet<CountryPricing> CountryPricing { get; set; }
    public DbSet<TaxConfiguration> TaxConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}