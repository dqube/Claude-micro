using BuildingBlocks.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Entities;

public class SupplierContactNumber : Entity<ContactNumberId>
{
    public ContactId ContactId { get; private set; } = null!;
    public int ContactNumberTypeId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private SupplierContactNumber() { } // EF Core

    public SupplierContactNumber(
        ContactNumberId id,
        ContactId contactId,
        int contactNumberTypeId,
        string phoneNumber,
        bool isPrimary = false,
        string? notes = null,
        Guid? createdBy = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        ContactId = contactId ?? throw new ArgumentNullException(nameof(contactId));
        ContactNumberTypeId = contactNumberTypeId;
        PhoneNumber = phoneNumber.Trim();
        IsPrimary = isPrimary;
        Notes = notes?.Trim();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    public void UpdateInfo(string phoneNumber, string? notes, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        PhoneNumber = phoneNumber.Trim();
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
} 