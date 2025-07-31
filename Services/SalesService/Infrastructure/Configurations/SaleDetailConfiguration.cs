using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Configurations;

public class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
{
    public void Configure(EntityTypeBuilder<SaleDetail> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("SaleDetails", "sales");
        builder.HasKey(sd => sd.Id);

        builder.Property(sd => sd.SaleId)
            .IsRequired();

        builder.Property(sd => sd.ProductId)
            .IsRequired();

        builder.Property(sd => sd.Quantity)
            .IsRequired();

        builder.Property(sd => sd.UnitPrice)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(sd => sd.AppliedDiscount)
            .HasColumnType("DECIMAL(19,4)")
            .HasDefaultValue(0);

        builder.Property(sd => sd.TaxApplied)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(sd => sd.LineTotal)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(sd => sd.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(sd => sd.CreatedBy);

        builder.HasIndex(sd => sd.SaleId)
            .HasDatabaseName("IX_SaleDetails_Sale");

        builder.HasIndex(sd => sd.ProductId)
            .HasDatabaseName("IX_SaleDetails_Product");
    }
}