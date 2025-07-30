using EmployeeService.Domain.Entities;
using EmployeeService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeService.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("EmployeeId")
            .HasConversion(
                employeeId => employeeId.Value,
                value => new EmployeeId(value))
            .ValueGeneratedNever();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.StoreId)
            .IsRequired();

        builder.Property(e => e.EmployeeNumber)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                employeeNumber => employeeNumber.Value,
                value => new EmployeeNumber(value));

        builder.Property(e => e.HireDate)
            .IsRequired();

        builder.Property(e => e.TerminationDate)
            .IsRequired(false);

        builder.Property(e => e.Position)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                position => position.Value,
                value => new Position(value));

        builder.Property(e => e.AuthLevel)
            .IsRequired();

        builder.HasMany(e => e.ContactNumbers)
            .WithOne()
            .HasForeignKey(cn => cn.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Addresses)
            .WithOne()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique();

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.Ignore(e => e.DomainEvents);
        builder.Ignore(e => e.IsActive);
    }
}