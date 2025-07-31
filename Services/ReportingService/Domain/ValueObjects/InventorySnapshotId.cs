using BuildingBlocks.Domain.StronglyTypedIds;

namespace ReportingService.Domain.ValueObjects;

public class InventorySnapshotId : StronglyTypedId<Guid>
{
    public InventorySnapshotId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("InventorySnapshotId cannot be empty", nameof(value));
    }
    
    public static InventorySnapshotId New() => new(Guid.NewGuid());
    
    public static InventorySnapshotId From(Guid value) => new(value);
} 