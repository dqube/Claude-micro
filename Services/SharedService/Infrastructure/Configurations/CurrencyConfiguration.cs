using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Entities;
using SharedService.Domain.ValueObjects;

namespace SharedService.Infrastructure.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies", "shared");

        // Primary key configuration
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(
                currencyCode => currencyCode.Value,
                value => CurrencyCode.From(value))
            .HasColumnName("CurrencyCode")
            .HasMaxLength(3)
            .IsRequired();

        // Value object configurations
        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => new CurrencyName(value))
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Symbol)
            .HasConversion(
                symbol => symbol.Value,
                value => new CurrencySymbol(value))
            .HasColumnName("Symbol")
            .HasMaxLength(5)
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

        // Indexes
        builder.HasIndex(c => c.Name);
    }
} 