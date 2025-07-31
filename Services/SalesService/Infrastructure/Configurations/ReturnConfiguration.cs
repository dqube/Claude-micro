using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Configurations;

public class ReturnConfiguration : IEntityTypeConfiguration<Return>
{
    public void Configure(EntityTypeBuilder<Return> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Returns", "sales");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.SaleId)
            .IsRequired();

        builder.Property(r => r.ReturnDate)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(r => r.EmployeeId)
            .IsRequired();

        builder.Property(r => r.CustomerId);

        builder.Property(r => r.TotalRefund)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(r => r.CreatedBy);

        builder.HasMany(r => r.ReturnDetails)
            .WithOne()
            .HasForeignKey(rd => rd.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.SaleId)
            .HasDatabaseName("IX_Returns_Sale");

        builder.HasIndex(r => r.ReturnDate)
            .HasDatabaseName("IX_Returns_Date");

        builder.HasIndex(r => r.EmployeeId)
            .HasDatabaseName("IX_Returns_Employee");

        builder.HasIndex(r => r.CustomerId)
            .HasDatabaseName("IX_Returns_Customer")
            .HasFilter("[CustomerId] IS NOT NULL");
    }
}