using EmployeeService.Domain.Entities;
using EmployeeService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeService.Infrastructure.Configurations;

public class EmployeeContactNumberConfiguration : IEntityTypeConfiguration<EmployeeContactNumber>
{
    public void Configure(EntityTypeBuilder<EmployeeContactNumber> builder)
    {
        builder.ToTable("EmployeeContactNumbers");

        builder.HasKey(cn => cn.Id);

        builder.Property(cn => cn.Id)
            .HasColumnName("ContactNumberId")
            .HasConversion(
                contactNumberId => contactNumberId.Value,
                value => ContactNumberId.From(value))
            .ValueGeneratedNever();

        builder.Property(cn => cn.EmployeeId)
            .IsRequired()
            .HasConversion(
                employeeId => employeeId.Value,
                value => EmployeeId.From(value));

        builder.Property(cn => cn.ContactNumberTypeId)
            .IsRequired();

        builder.Property(cn => cn.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(cn => cn.IsPrimary)
            .IsRequired();

        builder.Property(cn => cn.Verified)
            .IsRequired();
    }
}