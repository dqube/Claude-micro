using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "catalog");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("ProductId");

        builder.Property(p => p.SKU)
            .HasConversion(
                sku => sku.Value,
                value => SKU.From(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("IX_Products_SKU");

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnType("TEXT");

        builder.Property(p => p.CategoryId)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value))
            .IsRequired();

        builder.Property(p => p.BasePrice)
            .HasConversion(
                price => price.Value,
                value => Price.From(value))
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(p => p.CostPrice)
            .HasConversion(
                price => price.Value,
                value => Price.From(value))
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(p => p.IsTaxable)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.CreatedBy);

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.UpdatedBy);

        // Navigation properties
        builder.HasMany(p => p.Barcodes)
            .WithOne()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CountryPricing)
            .WithOne()
            .HasForeignKey(cp => cp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_Category");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Products_Name");
    }
}