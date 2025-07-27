using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class TokenId : StronglyTypedId<Guid>
{
    public TokenId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("TokenId cannot be empty", nameof(value));
    }
    
    public static TokenId New() => new(Guid.NewGuid());
    
    public static TokenId From(Guid value) => new(value);
}