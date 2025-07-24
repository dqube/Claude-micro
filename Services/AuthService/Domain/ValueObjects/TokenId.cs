using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class TokenId : GuidId
{
    public TokenId(Guid value) : base(value) { }
    
    public static TokenId New() => new(Guid.NewGuid());
    
    public static TokenId From(Guid value) => new(value);
}