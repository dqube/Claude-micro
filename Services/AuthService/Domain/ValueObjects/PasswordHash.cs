using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

public class PasswordHash : SingleValueObject<byte[]>
{
    public PasswordHash(byte[] value) : base(value)
    {
        if (value == null || value.Length == 0)
            throw new ArgumentException("Password hash cannot be null or empty", nameof(value));
    }

    public static PasswordHash From(byte[] hash) => new(hash);
    public static PasswordHash From(string hash, byte[] salt) => new(System.Text.Encoding.UTF8.GetBytes(hash));
    
    public string Hash => System.Text.Encoding.UTF8.GetString(Value);
    public byte[] Salt { get; private set; } = Array.Empty<byte>();
    
    private PasswordHash(byte[] hash, byte[] salt) : base(hash)
    {
        if (hash == null || hash.Length == 0)
            throw new ArgumentException("Password hash cannot be null or empty", nameof(hash));
        Salt = salt ?? throw new ArgumentNullException(nameof(salt));
    }
    
    public static PasswordHash Create(byte[] hash, byte[] salt) => new(hash, salt);
}