using BuildingBlocks.Domain.StronglyTypedIds;

namespace ReportingService.Domain.ValueObjects;

public class SalesSnapshotId : StronglyTypedId<Guid>
{
    public SalesSnapshotId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SalesSnapshotId cannot be empty", nameof(value));
    }
    
    public static SalesSnapshotId New() => new(Guid.NewGuid());
    
    public static SalesSnapshotId From(Guid value) => new(value);
} 