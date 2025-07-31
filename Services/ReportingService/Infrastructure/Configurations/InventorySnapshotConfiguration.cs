using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Common;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Infrastructure.Converters;

namespace ReportingService.Infrastructure.Configurations;

public class InventorySnapshotConfiguration : IEntityTypeConfiguration<InventorySnapshot>
{
    public void Configure(EntityTypeBuilder<InventorySnapshot> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("InventorySnapshots", "reporting");
        builder.HasKey(i => i.Id);

        // ProductId
        builder.Property(i => i.ProductId)
            .HasConversion(
                productId => productId.Value,
                value => ProductId.From(value))
            .IsRequired();

        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("IX_InventorySnapshots_ProductId");

        // StoreId
        builder.Property(i => i.StoreId)
            .HasConversion(
                storeId => storeId.Value,
                value => StoreId.From(value))
            .IsRequired();

        builder.HasIndex(i => i.StoreId)
            .HasDatabaseName("IX_InventorySnapshots_StoreId");

        // Quantity
        builder.Property(i => i.Quantity)
            .IsRequired();

        // SnapshotDate
        builder.Property(i => i.SnapshotDate)
            .HasConversion<DateOnlyConverter>()
            .HasDefaultValueSql("CAST(GETDATE() AS DATE)")
            .IsRequired();

        builder.HasIndex(i => i.SnapshotDate)
            .HasDatabaseName("IX_InventorySnapshots_SnapshotDate");

        // Audit fields
        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(i => i.CreatedBy);

        // Additional composite indexes for better performance
        builder.HasIndex(i => new { i.ProductId, i.StoreId })
            .HasDatabaseName("IX_InventorySnapshots_Product_Store");

        builder.HasIndex(i => new { i.StoreId, i.SnapshotDate })
            .HasDatabaseName("IX_InventorySnapshots_Store_Date");

        builder.HasIndex(i => new { i.ProductId, i.StoreId, i.SnapshotDate })
            .HasDatabaseName("IX_InventorySnapshots_Product_Store_Date");

        // Index for low inventory queries
        builder.HasIndex(i => new { i.StoreId, i.Quantity })
            .HasDatabaseName("IX_InventorySnapshots_Store_Quantity");
    }
} 