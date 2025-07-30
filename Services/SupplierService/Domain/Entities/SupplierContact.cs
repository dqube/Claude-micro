using BuildingBlocks.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Entities;

public class SupplierContact : Entity<ContactId>
{
    private readonly List<SupplierContactNumber> _contactNumbers = new();

    public SupplierId SupplierId { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Position { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyList<SupplierContactNumber> ContactNumbers => _contactNumbers.AsReadOnly();

    private SupplierContact() { } // EF Core

    public SupplierContact(
        ContactId id,
        SupplierId supplierId,
        string firstName,
        string lastName,
        string? email = null,
        string? position = null,
        bool isPrimary = false,
        string? notes = null,
        Guid? createdBy = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim();
        Position = position?.Trim();
        IsPrimary = isPrimary;
        Notes = notes?.Trim();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    public void UpdateInfo(string firstName, string lastName, string? email, string? position, string? notes, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim();
        Position = position?.Trim();
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void AddContactNumber(SupplierContactNumber contactNumber)
    {
        ArgumentNullException.ThrowIfNull(contactNumber);
        
        if (contactNumber.IsPrimary && _contactNumbers.Any(cn => cn.IsPrimary))
        {
            var currentPrimary = _contactNumbers.First(cn => cn.IsPrimary);
            currentPrimary.SetPrimary(false);
        }

        _contactNumbers.Add(contactNumber);
    }

    public void RemoveContactNumber(ContactNumberId contactNumberId)
    {
        var contactNumber = _contactNumbers.FirstOrDefault(cn => cn.Id == contactNumberId);
        if (contactNumber != null)
        {
            _contactNumbers.Remove(contactNumber);
        }
    }

    public string FullName => $"{FirstName} {LastName}";
} 