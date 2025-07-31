using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Infrastructure.Configurations;

public class ReturnDetailConfiguration : IEntityTypeConfiguration<ReturnDetail>
{
    public void Configure(EntityTypeBuilder<ReturnDetail> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("ReturnDetails", "sales");
        builder.HasKey(rd => rd.Id);

        builder.Property(rd => rd.ReturnId)
            .IsRequired();

        builder.Property(rd => rd.ProductId)
            .IsRequired();

        builder.Property(rd => rd.Quantity)
            .IsRequired();

        builder.Property(rd => rd.Reason)
            .HasConversion(
                reason => reason.Name,
                value => ReturnReason.FromName(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(rd => rd.Restock)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(rd => rd.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(rd => rd.CreatedBy);

        builder.HasIndex(rd => rd.ReturnId)
            .HasDatabaseName("IX_ReturnDetails_Return");

        builder.HasIndex(rd => rd.ProductId)
            .HasDatabaseName("IX_ReturnDetails_Product");
    }
}