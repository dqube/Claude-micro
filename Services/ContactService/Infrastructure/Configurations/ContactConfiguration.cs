using BuildingBlocks.Infrastructure.Data.Configurations;
using ContactService.Domain.Entities;
using ContactService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactService.Infrastructure.Configurations;

public class ContactConfiguration : EntityConfigurationBase<Contact, ContactId>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnName("Email");
        });

        builder.OwnsOne(c => c.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasMaxLength(20)
                .HasColumnName("PhoneNumber");
        });

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(200)
                .HasColumnName("AddressStreet");

            address.Property(a => a.City)
                .HasMaxLength(100)
                .HasColumnName("AddressCity");

            address.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("AddressPostalCode");

            address.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("AddressCountry");
        });

        builder.OwnsOne(c => c.ContactType, contactType =>
        {
            contactType.Property(ct => ct.Id)
                .HasColumnName("ContactTypeId");

            contactType.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ContactTypeName");
        });

        builder.Property(c => c.Company)
            .HasMaxLength(200);

        builder.Property(c => c.JobTitle)
            .HasMaxLength(100);

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.HasIndex(c => c.Email!.Value)
            .IsUnique()
            .HasDatabaseName("IX_Contacts_Email");

        builder.HasIndex(c => new { c.FirstName, c.LastName })
            .HasDatabaseName("IX_Contacts_Name");

        builder.HasIndex(c => c.ContactType!.Name)
            .HasDatabaseName("IX_Contacts_ContactType");
    }
}