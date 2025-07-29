using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Entities;

public class CustomerContactNumber : Entity<ContactNumberId>
{
    public CustomerId CustomerId { get; private set; }
    public int ContactNumberTypeId { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool Verified { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public CustomerId? UpdatedBy { get; private set; }

    // Navigation property
    public Customer Customer { get; private set; } = null!;

    // Private constructor for EF Core
    private CustomerContactNumber() : base(ContactNumberId.New())
    {
        CustomerId = CustomerId.New();
        PhoneNumber = new PhoneNumber("+1234567890");
    }

    public CustomerContactNumber(
        ContactNumberId id,
        CustomerId customerId,
        int contactNumberTypeId,
        PhoneNumber phoneNumber,
        bool isPrimary = false) : base(id)
    {
        CustomerId = customerId;
        ContactNumberTypeId = contactNumberTypeId;
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        IsPrimary = isPrimary;
        Verified = false;
        CreatedAt = DateTime.UtcNow;

        ValidateContactNumber();
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsVerified(CustomerId verifiedBy)
    {
        Verified = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = verifiedBy;
    }

    public void UpdatePhoneNumber(PhoneNumber newPhoneNumber, CustomerId updatedBy)
    {
        PhoneNumber = newPhoneNumber ?? throw new ArgumentNullException(nameof(newPhoneNumber));
        Verified = false; // Reset verification when phone number changes
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        ValidateContactNumber();
    }

    private void ValidateContactNumber()
    {
        if (ContactNumberTypeId <= 0)
            throw new ArgumentException("Contact number type ID must be positive", nameof(ContactNumberTypeId));
    }
} 