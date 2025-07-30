using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Entities;
using SharedService.Domain.ValueObjects;

namespace SharedService.Infrastructure.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries", "shared");

        // Primary key configuration
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(
                countryCode => countryCode.Value,
                value => CountryCode.From(value))
            .HasColumnName("CountryCode")
            .HasMaxLength(2)
            .IsRequired();

        // Value object configurations
        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => new CountryName(value))
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CurrencyCode)
            .HasConversion(
                currencyCode => currencyCode.Value,
                value => CurrencyCode.From(value))
            .HasColumnName("CurrencyCode")
            .HasMaxLength(3)
            .IsRequired();

        // Audit properties
        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasColumnName("CreatedBy");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(c => c.UpdatedBy)
            .HasColumnName("UpdatedBy");

        // Foreign key relationship
        builder.HasOne(c => c.Currency)
            .WithMany()
            .HasForeignKey(c => c.CurrencyCode)
            .HasPrincipalKey(cur => cur.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.CurrencyCode);
    }
} 