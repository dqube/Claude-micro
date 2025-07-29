using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using CustomerService.Domain.Events;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Entities;

public class Customer : AggregateRoot<CustomerId>
{
    public Guid? UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email? Email { get; private set; }
    public MembershipNumber MembershipNumber { get; private set; }
    public DateTime JoinDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string CountryCode { get; private set; }
    public int LoyaltyPoints { get; private set; }
    public int? PreferredContactMethod { get; private set; }
    public int? PreferredAddressType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public CustomerId? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public CustomerId? UpdatedBy { get; private set; }

    // Navigation properties
    private readonly List<CustomerContactNumber> _contactNumbers = new();
    private readonly List<CustomerAddress> _addresses = new();
    
    public IReadOnlyList<CustomerContactNumber> ContactNumbers => _contactNumbers.AsReadOnly();
    public IReadOnlyList<CustomerAddress> Addresses => _addresses.AsReadOnly();

    // Private constructor for EF Core
    private Customer() : base(CustomerId.New())
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MembershipNumber = MembershipNumber.Generate();
        CountryCode = string.Empty;
    }

    public Customer(
        CustomerId id,
        string firstName,
        string lastName,
        Email? email,
        MembershipNumber membershipNumber,
        DateTime joinDate,
        DateTime expiryDate,
        string countryCode,
        Guid? userId = null) : base(id)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email;
        MembershipNumber = membershipNumber ?? throw new ArgumentNullException(nameof(membershipNumber));
        JoinDate = joinDate;
        ExpiryDate = expiryDate;
        CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
        UserId = userId;
        LoyaltyPoints = 0;
        CreatedAt = DateTime.UtcNow;

        ValidateCustomer();

        AddDomainEvent(new CustomerCreatedEvent(Id, FirstName, LastName, Email, MembershipNumber, JoinDate));
    }

    public void UpdateEmail(Email newEmail, CustomerId updatedBy)
    {
        var oldEmail = Email;
        Email = newEmail;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        if (oldEmail != null && newEmail != null && !oldEmail.Equals(newEmail))
        {
            AddDomainEvent(new CustomerEmailUpdatedEvent(Id, oldEmail, newEmail));
        }
    }

    public void UpdatePersonalInfo(string firstName, string lastName, CustomerId updatedBy)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        ValidatePersonalInfo();
    }

    public void AddLoyaltyPoints(int points, string reason)
    {
        if (points <= 0)
            throw new ArgumentException("Points to add must be positive", nameof(points));

        var previousPoints = LoyaltyPoints;
        LoyaltyPoints += points;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LoyaltyPointsUpdatedEvent(Id, previousPoints, LoyaltyPoints, reason));
    }

    public void RedeemLoyaltyPoints(int points, string reason)
    {
        if (points <= 0)
            throw new ArgumentException("Points to redeem must be positive", nameof(points));

        if (LoyaltyPoints < points)
            throw new InvalidOperationException($"Insufficient loyalty points. Available: {LoyaltyPoints}, Required: {points}");

        var previousPoints = LoyaltyPoints;
        LoyaltyPoints -= points;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LoyaltyPointsUpdatedEvent(Id, previousPoints, LoyaltyPoints, reason));
    }

    public void AddContactNumber(CustomerContactNumber contactNumber)
    {
        ArgumentNullException.ThrowIfNull(contactNumber);
        
        // If this is being set as primary, ensure no other contact number is primary
        if (contactNumber.IsPrimary)
        {
            foreach (var existing in _contactNumbers)
            {
                existing.SetAsPrimary(false);
            }
        }

        _contactNumbers.Add(contactNumber);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAddress(CustomerAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        
        // If this is being set as primary, ensure no other address is primary
        if (address.IsPrimary)
        {
            foreach (var existing in _addresses)
            {
                existing.SetAsPrimary(false);
            }
        }

        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ExtendMembership(DateTime newExpiryDate, CustomerId updatedBy)
    {
        if (newExpiryDate <= ExpiryDate)
            throw new ArgumentException("New expiry date must be later than current expiry date", nameof(newExpiryDate));

        ExpiryDate = newExpiryDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public bool IsMembershipActive()
    {
        return ExpiryDate > DateTime.UtcNow;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    private void ValidateCustomer()
    {
        ValidatePersonalInfo();

        if (ExpiryDate <= JoinDate)
            throw new ArgumentException("Expiry date must be after join date");

        if (string.IsNullOrWhiteSpace(CountryCode) || CountryCode.Length != 2)
            throw new ArgumentException("Country code must be a valid 2-character code");
    }

    private void ValidatePersonalInfo()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            throw new ArgumentException("First name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(LastName))
            throw new ArgumentException("Last name cannot be null or empty");

        if (FirstName.Length > 50)
            throw new ArgumentException("First name cannot exceed 50 characters");

        if (LastName.Length > 50)
            throw new ArgumentException("Last name cannot exceed 50 characters");
    }
} 