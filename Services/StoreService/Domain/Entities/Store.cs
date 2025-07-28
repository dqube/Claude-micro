using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.ValueObjects;
using StoreService.Domain.Events;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Entities;

public class Store : AggregateRoot<StoreId>
{
    public string Name { get; private set; }
    public int LocationId { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public string OpeningHours { get; private set; }
    public StoreStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private Store() : base(StoreId.From(1))
    {
        Name = string.Empty;
        Address = new Address("Unknown", "Unknown", "00000", "Unknown");
        Phone = new PhoneNumber("+1234567890");
        OpeningHours = string.Empty;
        Status = StoreStatus.Closed;
    }

    public Store(
        StoreId id,
        string name,
        int locationId,
        Address address,
        PhoneNumber phone,
        string openingHours) : base(id)
    {
        ValidateName(name);
        ValidateOpeningHours(openingHours);

        Name = name.Trim();
        LocationId = locationId;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        OpeningHours = openingHours.Trim();
        Status = StoreStatus.Active;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreCreatedEvent(Id, Name, Address));
    }

    public void UpdateStoreInformation(string name, Address address, PhoneNumber phone, string openingHours)
    {
        ValidateName(name);
        ValidateOpeningHours(openingHours);

        var oldName = Name;
        var oldAddress = Address;

        Name = name.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        OpeningHours = openingHours.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreInformationUpdatedEvent(Id, oldName, Name, oldAddress, Address));
    }

    public void ChangeStatus(StoreStatus newStatus)
    {
        if (Status.Equals(newStatus)) return;

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new StoreStatusChangedEvent(Id, oldStatus, newStatus));
    }

    public void SetMaintenance()
    {
        ChangeStatus(StoreStatus.Maintenance);
    }

    public void Open()
    {
        if (Status.Equals(StoreStatus.Maintenance))
            throw new InvalidOperationException("Cannot open store while in maintenance mode");

        ChangeStatus(StoreStatus.Active);
    }

    public void Close()
    {
        ChangeStatus(StoreStatus.Closed);
    }

    public bool IsOperational => Status.Equals(StoreStatus.Active);

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Store name cannot be null or empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Store name cannot exceed 100 characters", nameof(name));
    }

    private static void ValidateOpeningHours(string openingHours)
    {
        if (string.IsNullOrWhiteSpace(openingHours))
            throw new ArgumentException("Opening hours cannot be null or empty", nameof(openingHours));

        if (openingHours.Length > 100)
            throw new ArgumentException("Opening hours cannot exceed 100 characters", nameof(openingHours));
    }
} 