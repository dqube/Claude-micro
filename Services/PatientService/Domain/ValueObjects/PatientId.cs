using BuildingBlocks.Domain.StronglyTypedIds;

namespace PatientService.Domain.ValueObjects;

public class PatientId : StronglyTypedId<Guid>
{
    public PatientId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PatientId cannot be empty", nameof(value));
    }
    
    public static PatientId New() => new(Guid.NewGuid());
    
    public static PatientId From(Guid value) => new(value);
}