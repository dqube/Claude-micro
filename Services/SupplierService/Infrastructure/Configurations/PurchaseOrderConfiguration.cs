using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders", "supplier");

        builder.HasKey(po => po.Id);

        builder.Property(po => po.Id)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value))
            .ValueGeneratedNever();

        builder.Property(po => po.SupplierId)
            .HasConversion(
                id => id.Value,
                value => SupplierId.From(value))
            .IsRequired();

        builder.Property(po => po.StoreId)
            .IsRequired();

        builder.Property(po => po.OrderDate)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(po => po.ExpectedDate);

        builder.Property(po => po.Status)
            .HasConversion(
                status => status.Value,
                value => PurchaseOrderStatus.From(value))
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(PurchaseOrderStatus.Draft);

        builder.Property(po => po.TotalAmount)
            .HasPrecision(19, 4)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(po => po.ShippingAddressId)
            .HasConversion(
                id => id!.Value,
                value => AddressId.From(value));

        builder.Property(po => po.ContactPersonId)
            .HasConversion(
                id => id!.Value,
                value => ContactId.From(value));

        // Configure audit properties
        builder.Property(po => po.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(po => po.CreatedBy);
        builder.Property(po => po.UpdatedAt);
        builder.Property(po => po.UpdatedBy);

        // Configure relationships
        builder.HasMany(po => po.OrderDetails)
            .WithOne()
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(po => po.SupplierId);
        builder.HasIndex(po => po.StoreId);
        builder.HasIndex(po => po.Status);
        builder.HasIndex(po => po.OrderDate);
    }
} 