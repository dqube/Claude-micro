using BuildingBlocks.Domain.ValueObjects;

namespace CustomerService.Domain.ValueObjects;

public class MembershipNumber : SingleValueObject<string>
{
    public MembershipNumber(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Membership number cannot be null or empty", nameof(value));
        
        if (value.Length < 6 || value.Length > 50)
            throw new ArgumentException("Membership number must be between 6 and 50 characters", nameof(value));
        
        if (!IsValidFormat(value))
            throw new ArgumentException("Membership number format is invalid", nameof(value));
    }
    
    public static MembershipNumber From(string value) => new(value);
    
    public static MembershipNumber Generate() => new($"MEM{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}");
    
    private static bool IsValidFormat(string value)
    {
        // Basic validation - can be enhanced based on business rules
        return value.All(c => char.IsLetterOrDigit(c) || c == '-');
    }
} 