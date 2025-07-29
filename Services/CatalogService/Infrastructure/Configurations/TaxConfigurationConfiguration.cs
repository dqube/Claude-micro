using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Configurations;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfigurations", "catalog");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Id)
            .HasConversion(
                id => id.Value,
                value => TaxConfigId.From(value))
            .HasColumnName("TaxConfigId");

        builder.Property(tc => tc.LocationId)
            .IsRequired();

        builder.Property(tc => tc.CategoryId)
            .HasConversion(
                id => id != null ? id.Value : (int?)null,
                value => value.HasValue ? CategoryId.From(value.Value) : null);

        builder.Property(tc => tc.TaxRate)
            .HasConversion(
                rate => rate.Value,
                value => TaxRate.From(value))
            .HasColumnType("DECIMAL(5,2)")
            .IsRequired();

        builder.Property(tc => tc.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(tc => tc.CreatedBy);

        builder.Property(tc => tc.UpdatedAt);

        builder.Property(tc => tc.UpdatedBy);

        // Index for location queries
        builder.HasIndex(tc => tc.LocationId)
            .HasDatabaseName("IX_TaxConfigurations_Location");

        // Index for location and category queries
        builder.HasIndex(tc => new { tc.LocationId, tc.CategoryId })
            .HasDatabaseName("IX_TaxConfigurations_LocationCategory");
    }
}