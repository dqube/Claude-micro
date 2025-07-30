using BuildingBlocks.Domain.Entities;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Entities;

public class EmployeeAddress : Entity<AddressId>
{
    public EmployeeId EmployeeId { get; private set; }
    public int AddressTypeId { get; private set; }
    public string Line1 { get; private set; }
    public string? Line2 { get; private set; }
    public string City { get; private set; }
    public string? State { get; private set; }
    public string PostalCode { get; private set; }
    public string CountryCode { get; private set; }
    public bool IsPrimary { get; private set; }

    private EmployeeAddress() { }

    public EmployeeAddress(
        AddressId id,
        EmployeeId employeeId,
        int addressTypeId,
        string line1,
        string city,
        string postalCode,
        string countryCode,
        string? line2 = null,
        string? state = null,
        bool isPrimary = false) : base(id)
    {
        if (employeeId == null || employeeId.Value == Guid.Empty)
            throw new ArgumentException("EmployeeId cannot be empty", nameof(employeeId));
        if (addressTypeId <= 0)
            throw new ArgumentException("AddressTypeId must be greater than 0", nameof(addressTypeId));
        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Line1 cannot be null or empty", nameof(line1));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("PostalCode cannot be null or empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("CountryCode cannot be null or empty", nameof(countryCode));
        if (line1.Length > 100)
            throw new ArgumentException("Line1 cannot exceed 100 characters", nameof(line1));
        if (city.Length > 50)
            throw new ArgumentException("City cannot exceed 50 characters", nameof(city));
        if (postalCode.Length > 20)
            throw new ArgumentException("PostalCode cannot exceed 20 characters", nameof(postalCode));
        if (countryCode.Length != 2)
            throw new ArgumentException("CountryCode must be exactly 2 characters", nameof(countryCode));

        EmployeeId = employeeId;
        AddressTypeId = addressTypeId;
        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        PostalCode = postalCode;
        CountryCode = countryCode;
        IsPrimary = isPrimary;
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void UpdateAddress(
        string line1,
        string city,
        string postalCode,
        string countryCode,
        string? line2 = null,
        string? state = null)
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Line1 cannot be null or empty", nameof(line1));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("PostalCode cannot be null or empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("CountryCode cannot be null or empty", nameof(countryCode));

        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }
}