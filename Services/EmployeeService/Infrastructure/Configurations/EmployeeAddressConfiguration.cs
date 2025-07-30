using EmployeeService.Domain.Entities;
using EmployeeService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeService.Infrastructure.Configurations;

public class EmployeeAddressConfiguration : IEntityTypeConfiguration<EmployeeAddress>
{
    public void Configure(EntityTypeBuilder<EmployeeAddress> builder)
    {
        builder.ToTable("EmployeeAddresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("AddressId")
            .HasConversion(
                addressId => addressId.Value,
                value => AddressId.From(value))
            .ValueGeneratedNever();

        builder.Property(a => a.EmployeeId)
            .IsRequired()
            .HasConversion(
                employeeId => employeeId.Value,
                value => EmployeeId.From(value));

        builder.Property(a => a.AddressTypeId)
            .IsRequired();

        builder.Property(a => a.Line1)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Line2)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.City)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.State)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.CountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(a => a.IsPrimary)
            .IsRequired();
    }
}