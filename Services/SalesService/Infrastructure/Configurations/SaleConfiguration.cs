using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Infrastructure.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Sales", "sales");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StoreId)
            .IsRequired();

        builder.Property(s => s.EmployeeId)
            .IsRequired();

        builder.Property(s => s.CustomerId);

        builder.Property(s => s.RegisterId)
            .IsRequired();

        builder.Property(s => s.TransactionTime)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(s => s.SubTotal)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(s => s.DiscountTotal)
            .HasColumnType("DECIMAL(19,4)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(s => s.TaxAmount)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(s => s.TotalAmount)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(s => s.ReceiptNumber)
            .HasConversion(
                receiptNumber => receiptNumber.Value,
                value => ReceiptNumber.From(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(s => s.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_Sales_ReceiptNumber");

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(s => s.CreatedBy);

        builder.HasMany(s => s.SaleDetails)
            .WithOne()
            .HasForeignKey(sd => sd.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.AppliedDiscounts)
            .WithOne()
            .HasForeignKey(ad => ad.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.TransactionTime, s.StoreId })
            .HasDatabaseName("IX_Sales_DateStore");

        builder.HasIndex(s => s.EmployeeId)
            .HasDatabaseName("IX_Sales_EmployeeId");

        builder.HasIndex(s => s.CustomerId)
            .HasDatabaseName("IX_Sales_CustomerId")
            .HasFilter("[CustomerId] IS NOT NULL");
    }
}