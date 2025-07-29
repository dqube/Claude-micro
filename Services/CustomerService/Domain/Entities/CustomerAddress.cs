using BuildingBlocks.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Entities;

public class CustomerAddress : Entity<CustomerAddressId>
{
    public CustomerId CustomerId { get; private set; }
    public int AddressTypeId { get; private set; }
    public string Line1 { get; private set; }
    public string? Line2 { get; private set; }
    public string City { get; private set; }
    public string? State { get; private set; }
    public string PostalCode { get; private set; }
    public string CountryCode { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public CustomerId? UpdatedBy { get; private set; }

    // Navigation property
    public Customer Customer { get; private set; } = null!;

    // Private constructor for EF Core
    private CustomerAddress() : base(CustomerAddressId.New())
    {
        CustomerId = CustomerId.New();
        Line1 = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;
        CountryCode = string.Empty;
    }

    public CustomerAddress(
        CustomerAddressId id,
        CustomerId customerId,
        int addressTypeId,
        string line1,
        string city,
        string postalCode,
        string countryCode,
        string? line2 = null,
        string? state = null,
        bool isPrimary = false) : base(id)
    {
        CustomerId = customerId;
        AddressTypeId = addressTypeId;
        Line1 = line1 ?? throw new ArgumentNullException(nameof(line1));
        Line2 = line2;
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state;
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;

        ValidateAddress();
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(
        string line1,
        string city,
        string postalCode,
        string countryCode,
        CustomerId updatedBy,
        string? line2 = null,
        string? state = null)
    {
        Line1 = line1 ?? throw new ArgumentNullException(nameof(line1));
        Line2 = line2;
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state;
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        ValidateAddress();
    }

    public string GetFormattedAddress()
    {
        var address = Line1;
        if (!string.IsNullOrWhiteSpace(Line2))
            address += $", {Line2}";
        
        address += $", {City}";
        
        if (!string.IsNullOrWhiteSpace(State))
            address += $", {State}";
        
        address += $" {PostalCode}, {CountryCode}";
        
        return address;
    }

    private void ValidateAddress()
    {
        if (AddressTypeId <= 0)
            throw new ArgumentException("Address type ID must be positive", nameof(AddressTypeId));

        if (string.IsNullOrWhiteSpace(Line1) || Line1.Length > 100)
            throw new ArgumentException("Line1 must be between 1 and 100 characters", nameof(Line1));

        if (!string.IsNullOrEmpty(Line2) && Line2.Length > 100)
            throw new ArgumentException("Line2 cannot exceed 100 characters", nameof(Line2));

        if (string.IsNullOrWhiteSpace(City) || City.Length > 50)
            throw new ArgumentException("City must be between 1 and 50 characters", nameof(City));

        if (!string.IsNullOrEmpty(State) && State.Length > 50)
            throw new ArgumentException("State cannot exceed 50 characters", nameof(State));

        if (string.IsNullOrWhiteSpace(PostalCode) || PostalCode.Length > 20)
            throw new ArgumentException("Postal code must be between 1 and 20 characters", nameof(PostalCode));

        if (string.IsNullOrWhiteSpace(CountryCode) || CountryCode.Length != 2)
            throw new ArgumentException("Country code must be a valid 2-character code", nameof(CountryCode));
    }
} 