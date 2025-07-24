using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class UserId : GuidId
{
    public UserId(Guid value) : base(value) { }
    
    public static UserId New() => new(Guid.NewGuid());
    
    public static UserId From(Guid value) => new(value);
}