using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;

namespace PatientService.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        // Medical Record Number
        builder.Property(p => p.MedicalRecordNumber)
            .HasConversion(
                mrn => mrn.Value,
                value => new MedicalRecordNumber(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => p.MedicalRecordNumber)
            .IsUnique()
            .HasDatabaseName("IX_Patients_MedicalRecordNumber");

        // Patient Name
        builder.OwnsOne(p => p.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(n => n.MiddleName)
                .HasColumnName("MiddleName")
                .HasMaxLength(100);
        });

        // Email
        builder.Property(p => p.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(320)
            .IsRequired();

        // Phone Number
        builder.Property(p => p.PhoneNumber)
            .HasConversion(
                phone => phone != null ? phone.Value : null,
                value => value != null ? new PhoneNumber(value) : null)
            .HasMaxLength(20);

        // Address
        builder.OwnsOne(p => p.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100);

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20);

            addressBuilder.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(100);
        });

        // Date of Birth
        builder.Property(p => p.DateOfBirth)
            .HasColumnType("date")
            .IsRequired();

        // Gender
        builder.Property(p => p.Gender)
            .HasConversion(
                gender => gender.Id,
                id => Gender.FromId(id))
            .IsRequired();

        // Blood Type
        builder.Property(p => p.BloodType)
            .HasConversion(
                bloodType => bloodType != null ? bloodType.Id : (int?)null,
                id => id.HasValue ? BloodType.FromId(id.Value) : null);

        // Status and timestamps
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Indexes
        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Patients_Email");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Patients_IsActive");

        builder.HasIndex(p => p.DateOfBirth)
            .HasDatabaseName("IX_Patients_DateOfBirth");

        // Remove the composite index for now - it's causing issues with anonymous types
        // builder.HasIndex(p => new { p.Name.LastName, p.Name.FirstName })
        //     .HasDatabaseName("IX_Patients_Name");
    }
}