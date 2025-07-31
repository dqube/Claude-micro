using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Common;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Infrastructure.Converters;

namespace ReportingService.Infrastructure.Configurations;

public class SalesSnapshotConfiguration : IEntityTypeConfiguration<SalesSnapshot>
{
    public void Configure(EntityTypeBuilder<SalesSnapshot> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("SalesSnapshots", "reporting");
        builder.HasKey(s => s.Id);

        // SaleId
        builder.Property(s => s.SaleId)
            .HasConversion(
                saleId => saleId.Value,
                value => SaleId.From(value))
            .IsRequired();

        builder.HasIndex(s => s.SaleId)
            .HasDatabaseName("IX_SalesSnapshots_SaleId");

        // StoreId
        builder.Property(s => s.StoreId)
            .HasConversion(
                storeId => storeId.Value,
                value => StoreId.From(value))
            .IsRequired();

        builder.HasIndex(s => s.StoreId)
            .HasDatabaseName("IX_SalesSnapshots_StoreId");

        // SaleDate
        builder.Property(s => s.SaleDate)
            .HasConversion<DateOnlyConverter>()
            .IsRequired();

        builder.HasIndex(s => s.SaleDate)
            .HasDatabaseName("IX_SalesSnapshots_SaleDate");

        // TotalAmount
        builder.Property(s => s.TotalAmount)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        // CustomerId (nullable)
        builder.Property(s => s.CustomerId)
            .HasConversion(
                customerId => customerId != null ? customerId.Value : (Guid?)null,
                value => value.HasValue ? CustomerId.From(value.Value) : null);

        builder.HasIndex(s => s.CustomerId)
            .HasDatabaseName("IX_SalesSnapshots_CustomerId")
            .HasFilter("[CustomerId] IS NOT NULL");

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(s => s.CreatedBy);

        // Additional composite indexes for better performance
        builder.HasIndex(s => new { s.StoreId, s.SaleDate })
            .HasDatabaseName("IX_SalesSnapshots_Store_Date");

        builder.HasIndex(s => new { s.SaleDate, s.TotalAmount })
            .HasDatabaseName("IX_SalesSnapshots_Date_Amount");
    }
}

 