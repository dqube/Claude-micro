using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class SupplierContactAddedEvent : DomainEventBase
{
    public SupplierId SupplierId { get; }
    public ContactId ContactId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public SupplierContactAddedEvent(SupplierId supplierId, ContactId contactId, string firstName, string lastName)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        ContactId = contactId ?? throw new ArgumentNullException(nameof(contactId));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }
} 