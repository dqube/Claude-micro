using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class Username : ValueObject
{
    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be null or empty", nameof(value));
        
        if (value.Length < 3 || value.Length > 50)
            throw new ArgumentException("Username must be between 3 and 50 characters", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Username username)
    {
        ArgumentNullException.ThrowIfNull(username);
        return username.Value;
    }
    public static explicit operator Username(string value) => new(value);
}
