using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;
using SupplierService.Domain.Events;

namespace SupplierService.Domain.Entities;

public class Supplier : AggregateRoot<SupplierId>
{
    private readonly List<SupplierContact> _contacts = new();
    private readonly List<SupplierAddress> _addresses = new();
    private readonly List<PurchaseOrder> _purchaseOrders = new();

    public string Name { get; private set; } = string.Empty;
    public string? TaxIdentificationNumber { get; private set; }
    public string? Website { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyList<SupplierContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyList<SupplierAddress> Addresses => _addresses.AsReadOnly();
    public IReadOnlyList<PurchaseOrder> PurchaseOrders => _purchaseOrders.AsReadOnly();

    private Supplier() { } // EF Core

    public Supplier(
        SupplierId id,
        string name,
        string? taxIdentificationNumber = null,
        string? website = null,
        string? notes = null,
        Guid? createdBy = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name cannot be empty", nameof(name));

        Name = name.Trim();
        TaxIdentificationNumber = taxIdentificationNumber?.Trim();
        Website = website?.Trim();
        Notes = notes?.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        AddDomainEvent(new SupplierCreatedEvent(Id, Name));
    }

    public void UpdateBasicInfo(string name, string? taxIdentificationNumber, string? website, string? notes, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name cannot be empty", nameof(name));

        var oldName = Name;
        Name = name.Trim();
        TaxIdentificationNumber = taxIdentificationNumber?.Trim();
        Website = website?.Trim();
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        if (oldName != Name)
        {
            AddDomainEvent(new SupplierNameUpdatedEvent(Id, oldName, Name));
        }

        AddDomainEvent(new SupplierUpdatedEvent(Id, Name));
    }

    public void Activate(Guid updatedBy)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new SupplierActivatedEvent(Id, Name));
    }

    public void Deactivate(Guid updatedBy)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new SupplierDeactivatedEvent(Id, Name));
    }

    public void AddContact(SupplierContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        
        if (contact.IsPrimary && _contacts.Any(c => c.IsPrimary))
        {
            // Remove primary status from existing contacts
            var currentPrimary = _contacts.First(c => c.IsPrimary);
            currentPrimary.SetPrimary(false);
        }

        _contacts.Add(contact);
        AddDomainEvent(new SupplierContactAddedEvent(Id, contact.Id, contact.FirstName, contact.LastName));
    }

    public void RemoveContact(ContactId contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null) return;

        _contacts.Remove(contact);
        AddDomainEvent(new SupplierContactRemovedEvent(Id, contactId, contact.FirstName, contact.LastName));
    }

    public void AddAddress(SupplierAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        
        if (address.IsPrimary && _addresses.Any(a => a.IsPrimary))
        {
            // Remove primary status from existing addresses
            var currentPrimary = _addresses.First(a => a.IsPrimary);
            currentPrimary.SetPrimary(false);
        }

        _addresses.Add(address);
        AddDomainEvent(new SupplierAddressAddedEvent(Id, address.Id, address.Line1, address.City));
    }

    public void RemoveAddress(AddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null) return;

        _addresses.Remove(address);
        AddDomainEvent(new SupplierAddressRemovedEvent(Id, addressId, address.Line1, address.City));
    }

    public void AddPurchaseOrder(PurchaseOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _purchaseOrders.Add(order);
    }
} 