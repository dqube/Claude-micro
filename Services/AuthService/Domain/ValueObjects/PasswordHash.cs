using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class PasswordHash : SingleValueObject<byte[]>
{
    public PasswordHash(byte[] value) : base(value)
    {
        if (value == null || value.Length == 0)
            throw new ArgumentException("Password hash cannot be null or empty", nameof(value));
    }
}