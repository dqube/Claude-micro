using BuildingBlocks.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Entities;

public class SupplierAddress : Entity<AddressId>
{
    public SupplierId SupplierId { get; private set; } = null!;
    public int AddressTypeId { get; private set; }
    public string Line1 { get; private set; } = string.Empty;
    public string? Line2 { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string? State { get; private set; }
    public string PostalCode { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public bool IsShipping { get; private set; }
    public bool IsBilling { get; private set; }
    public string? Notes { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private SupplierAddress() { } // EF Core

    public SupplierAddress(
        AddressId id,
        SupplierId supplierId,
        int addressTypeId,
        string line1,
        string city,
        string postalCode,
        string countryCode,
        string? line2 = null,
        string? state = null,
        bool isPrimary = false,
        bool isShipping = false,
        bool isBilling = false,
        string? notes = null,
        Guid? createdBy = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Address line 1 cannot be empty", nameof(line1));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be empty", nameof(countryCode));

        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        AddressTypeId = addressTypeId;
        Line1 = line1.Trim();
        Line2 = line2?.Trim();
        City = city.Trim();
        State = state?.Trim();
        PostalCode = postalCode.Trim();
        CountryCode = countryCode.Trim();
        IsPrimary = isPrimary;
        IsShipping = isShipping;
        IsBilling = isBilling;
        Notes = notes?.Trim();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    public void UpdateInfo(
        string line1,
        string city,
        string postalCode,
        string countryCode,
        string? line2 = null,
        string? state = null,
        string? notes = null,
        Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Address line 1 cannot be empty", nameof(line1));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be empty", nameof(countryCode));

        Line1 = line1.Trim();
        Line2 = line2?.Trim();
        City = city.Trim();
        State = state?.Trim();
        PostalCode = postalCode.Trim();
        CountryCode = countryCode.Trim();
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;

        if (updatedBy.HasValue)
            UpdatedBy = updatedBy;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void SetShipping(bool isShipping)
    {
        IsShipping = isShipping;
    }

    public void SetBilling(bool isBilling)
    {
        IsBilling = isBilling;
    }

    public string FullAddress => $"{Line1}{(string.IsNullOrEmpty(Line2) ? "" : $", {Line2}")}, {City}{(string.IsNullOrEmpty(State) ? "" : $", {State}")} {PostalCode}, {CountryCode}";
} 