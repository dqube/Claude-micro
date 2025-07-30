using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class SupplierContactConfiguration : IEntityTypeConfiguration<SupplierContact>
{
    public void Configure(EntityTypeBuilder<SupplierContact> builder)
    {
        builder.ToTable("SupplierContacts", "supplier");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Id)
            .HasConversion(
                id => id.Value,
                value => ContactId.From(value))
            .ValueGeneratedNever();

        builder.Property(sc => sc.SupplierId)
            .HasConversion(
                id => id.Value,
                value => SupplierId.From(value))
            .IsRequired();

        builder.Property(sc => sc.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sc => sc.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sc => sc.Email)
            .HasMaxLength(100);

        builder.Property(sc => sc.Position)
            .HasMaxLength(100);

        builder.Property(sc => sc.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sc => sc.Notes)
            .HasColumnType("text");

        // Configure audit properties
        builder.Property(sc => sc.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(sc => sc.CreatedBy);
        builder.Property(sc => sc.UpdatedAt);
        builder.Property(sc => sc.UpdatedBy);

        // Configure relationships
        builder.HasMany(sc => sc.ContactNumbers)
            .WithOne()
            .HasForeignKey(cn => cn.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(sc => sc.SupplierId);
        builder.HasIndex(sc => sc.Email);
        builder.HasIndex(sc => sc.IsPrimary);
    }
} 