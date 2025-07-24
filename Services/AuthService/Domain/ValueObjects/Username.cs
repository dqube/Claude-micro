using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class Username : SingleValueObject<string>
{
    public Username(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be null or empty", nameof(value));
        
        if (value.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters long", nameof(value));
        
        if (value.Length > 50)
            throw new ArgumentException("Username cannot exceed 50 characters", nameof(value));
        
        if (!IsValidUsername(value))
            throw new ArgumentException("Username contains invalid characters", nameof(value));
    }
    
    private static bool IsValidUsername(string username)
    {
        return username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
    }
}