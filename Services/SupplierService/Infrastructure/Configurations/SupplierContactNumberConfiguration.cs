using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class SupplierContactNumberConfiguration : IEntityTypeConfiguration<SupplierContactNumber>
{
    public void Configure(EntityTypeBuilder<SupplierContactNumber> builder)
    {
        builder.ToTable("SupplierContactNumbers", "supplier");

        builder.HasKey(scn => scn.Id);

        builder.Property(scn => scn.Id)
            .HasConversion(
                id => id.Value,
                value => ContactNumberId.From(value))
            .ValueGeneratedNever();

        builder.Property(scn => scn.ContactId)
            .HasConversion(
                id => id.Value,
                value => ContactId.From(value))
            .IsRequired();

        builder.Property(scn => scn.ContactNumberTypeId)
            .IsRequired();

        builder.Property(scn => scn.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(scn => scn.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(scn => scn.Notes)
            .HasMaxLength(255);

        // Configure audit properties
        builder.Property(scn => scn.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(scn => scn.CreatedBy);
        builder.Property(scn => scn.UpdatedAt);
        builder.Property(scn => scn.UpdatedBy);

        // Indexes
        builder.HasIndex(scn => scn.ContactId);
        builder.HasIndex(scn => scn.IsPrimary);
    }
} 