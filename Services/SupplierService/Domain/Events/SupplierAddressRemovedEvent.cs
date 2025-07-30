using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class SupplierAddressRemovedEvent : DomainEventBase
{
    public SupplierId SupplierId { get; }
    public AddressId AddressId { get; }
    public string Line1 { get; }
    public string City { get; }

    public SupplierAddressRemovedEvent(SupplierId supplierId, AddressId addressId, string line1, string city)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        AddressId = addressId ?? throw new ArgumentNullException(nameof(addressId));
        Line1 = line1 ?? throw new ArgumentNullException(nameof(line1));
        City = city ?? throw new ArgumentNullException(nameof(city));
    }
} 