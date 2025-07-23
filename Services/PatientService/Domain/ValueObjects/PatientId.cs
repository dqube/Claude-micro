using BuildingBlocks.Domain.StronglyTypedIds;

namespace PatientService.Domain.ValueObjects;

public class PatientId : GuidId
{
    public PatientId(Guid value) : base(value) { }
    
    public static PatientId New() => new(Guid.NewGuid());
    
    public static PatientId From(Guid value) => new(value);
}