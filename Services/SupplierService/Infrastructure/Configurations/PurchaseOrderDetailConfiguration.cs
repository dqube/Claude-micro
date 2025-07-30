using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Infrastructure.Configurations;

public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        builder.ToTable("PurchaseOrderDetails", "supplier");

        builder.HasKey(pod => pod.Id);

        builder.Property(pod => pod.Id)
            .HasConversion(
                id => id.Value,
                value => OrderDetailId.From(value))
            .ValueGeneratedNever();

        builder.Property(pod => pod.OrderId)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value))
            .IsRequired();

        builder.Property(pod => pod.ProductId)
            .IsRequired();

        builder.Property(pod => pod.Quantity)
            .IsRequired();

        builder.Property(pod => pod.UnitCost)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(pod => pod.ReceivedQuantity);

        // Configure audit properties
        builder.Property(pod => pod.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(pod => pod.CreatedBy);

        // Indexes
        builder.HasIndex(pod => pod.OrderId);
        builder.HasIndex(pod => pod.ProductId);
    }
} 