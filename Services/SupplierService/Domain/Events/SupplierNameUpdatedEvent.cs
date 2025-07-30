using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class SupplierNameUpdatedEvent : DomainEventBase
{
    public SupplierId SupplierId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public SupplierNameUpdatedEvent(SupplierId supplierId, string oldName, string newName)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        OldName = oldName ?? throw new ArgumentNullException(nameof(oldName));
        NewName = newName ?? throw new ArgumentNullException(nameof(newName));
    }
} 