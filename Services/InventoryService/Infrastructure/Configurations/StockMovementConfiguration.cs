using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("StockMovements", "inventory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => StockMovementId.From(value))
            .HasColumnName("MovementId");


        builder.Property(x => x.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(x => x.StoreId)
            .HasConversion(
                id => id.Value,
                value => StoreId.From(value))
            .HasColumnName("StoreId")
            .IsRequired();

        builder.Property(x => x.QuantityChange)
            .HasColumnName("QuantityChange")
            .IsRequired();

        builder.Property(x => x.MovementType)
            .HasConversion(
                v => v.ToString(),
                v => MovementTypeValue.From(v))
            .HasColumnName("MovementType")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.MovementDate)
            .HasColumnName("MovementDate")
            .IsRequired();

        builder.Property(x => x.ReferenceId)
            .HasColumnName("ReferenceId");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? EmployeeId.From(value.Value) : null)
            .HasColumnName("CreatedBy");

        builder.HasIndex(x => x.MovementDate)
            .HasDatabaseName("IX_StockMovements_Date");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_StockMovements_Product");

        builder.HasIndex(x => x.StoreId)
            .HasDatabaseName("IX_StockMovements_Store");

    }
}