using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class RoleId : IntId
{
    public RoleId(int value) : base(value) { }
    
    public static RoleId From(int value) => new(value);
}