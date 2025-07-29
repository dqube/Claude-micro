using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Configurations;

public class ProductBarcodeConfiguration : IEntityTypeConfiguration<ProductBarcode>
{
    public void Configure(EntityTypeBuilder<ProductBarcode> builder)
    {
        builder.ToTable("ProductBarcodes", "catalog");

        builder.HasKey(pb => pb.Id);

        builder.Property(pb => pb.Id)
            .HasConversion(
                id => id.Value,
                value => BarcodeId.From(value))
            .HasColumnName("BarcodeId");

        builder.Property(pb => pb.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .IsRequired();

        builder.Property(pb => pb.BarcodeValue)
            .HasConversion(
                bv => bv.Value,
                value => BarcodeValue.From(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(pb => pb.BarcodeValue)
            .IsUnique()
            .HasDatabaseName("IX_ProductBarcodes_BarcodeValue");

        builder.Property(pb => pb.BarcodeType)
            .HasConversion(
                bt => bt.Name,
                value => BarcodeType.FromName(value))
            .HasMaxLength(20)
            .HasDefaultValue("UPC-A");

        builder.Property(pb => pb.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(pb => pb.CreatedBy);
    }
}