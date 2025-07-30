using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers", "supplier");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SupplierId.From(value))
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.TaxIdentificationNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Website)
            .HasMaxLength(255);

        builder.Property(s => s.Notes)
            .HasColumnType("text");

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure audit properties
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(s => s.CreatedBy);

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.UpdatedBy);

        // Configure relationships
        builder.HasMany(s => s.Contacts)
            .WithOne()
            .HasForeignKey(c => c.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Addresses)
            .WithOne()
            .HasForeignKey(a => a.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.PurchaseOrders)
            .WithOne()
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.TaxIdentificationNumber);
        builder.HasIndex(s => s.IsActive);
    }
} 