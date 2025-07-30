using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class SupplierAddressConfiguration : IEntityTypeConfiguration<SupplierAddress>
{
    public void Configure(EntityTypeBuilder<SupplierAddress> builder)
    {
        builder.ToTable("SupplierAddresses", "supplier");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Id)
            .HasConversion(
                id => id.Value,
                value => AddressId.From(value))
            .ValueGeneratedNever();

        builder.Property(sa => sa.SupplierId)
            .HasConversion(
                id => id.Value,
                value => SupplierId.From(value))
            .IsRequired();

        builder.Property(sa => sa.AddressTypeId)
            .IsRequired();

        builder.Property(sa => sa.Line1)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sa => sa.Line2)
            .HasMaxLength(100);

        builder.Property(sa => sa.City)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sa => sa.State)
            .HasMaxLength(50);

        builder.Property(sa => sa.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(sa => sa.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(sa => sa.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sa => sa.IsShipping)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sa => sa.IsBilling)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sa => sa.Notes)
            .HasColumnType("text");

        // Configure audit properties
        builder.Property(sa => sa.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(sa => sa.CreatedBy);
        builder.Property(sa => sa.UpdatedAt);
        builder.Property(sa => sa.UpdatedBy);

        // Indexes
        builder.HasIndex(sa => sa.SupplierId);
        builder.HasIndex(sa => sa.CountryCode);
        builder.HasIndex(sa => sa.IsPrimary);
        builder.HasIndex(sa => sa.IsShipping);
        builder.HasIndex(sa => sa.IsBilling);
    }
} 