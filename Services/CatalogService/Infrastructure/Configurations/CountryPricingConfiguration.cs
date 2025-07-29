using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Configurations;

public class CountryPricingConfiguration : IEntityTypeConfiguration<CountryPricing>
{
    public void Configure(EntityTypeBuilder<CountryPricing> builder)
    {
        builder.ToTable("CountryPricing", "catalog");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
            .HasConversion(
                id => id.Value,
                value => PricingId.From(value))
            .HasColumnName("PricingId");

        builder.Property(cp => cp.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .IsRequired();

        builder.Property(cp => cp.CountryCode)
            .HasConversion(
                cc => cc.Value,
                value => CountryCode.From(value))
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(cp => cp.Price)
            .HasConversion(
                price => price.Value,
                value => Price.From(value))
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(cp => cp.EffectiveDate)
            .HasColumnType("DATE")
            .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

        builder.Property(cp => cp.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(cp => cp.CreatedBy);

        // Unique constraint on ProductId and CountryCode
        builder.HasIndex(cp => new { cp.ProductId, cp.CountryCode })
            .IsUnique()
            .HasDatabaseName("IX_CountryPricing_ProductCountry");

        // Index for effective date queries
        builder.HasIndex(cp => cp.EffectiveDate)
            .HasDatabaseName("IX_CountryPricing_EffectiveDate");
    }
}