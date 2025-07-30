using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("InventoryItems", "inventory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => InventoryItemId.From(value))
            .HasColumnName("InventoryItemId");

        builder.Property(x => x.StoreId)
            .HasConversion(
                id => id.Value,
                value => StoreId.From(value))
            .HasColumnName("StoreId")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("Quantity")
            .IsRequired();

        builder.Property(x => x.ReorderLevel)
            .HasColumnName("ReorderLevel")
            .IsRequired();

        builder.Property(x => x.LastRestockDate)
            .HasColumnName("LastRestockDate");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? EmployeeId.From(value.Value) : null)
            .HasColumnName("CreatedBy");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(x => x.UpdatedBy)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? EmployeeId.From(value.Value) : null)
            .HasColumnName("UpdatedBy");

        builder.HasIndex(x => new { x.StoreId, x.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_InventoryItems_StoreProduct");

        builder.Ignore(x => x.DomainEvents);
    }
}