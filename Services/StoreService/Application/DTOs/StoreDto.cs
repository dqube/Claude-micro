namespace StoreService.Application.DTOs;

public record StoreDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int LocationId { get; init; }
    public AddressDto Address { get; init; } = null!;
    public string PhoneNumber { get; init; } = string.Empty;
    public string OpeningHours { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsOperational { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country);

public record RegisterDto
{
    public int Id { get; init; }
    public int StoreId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal CurrentBalance { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsOpen { get; init; }
    public DateTime? LastOpen { get; init; }
    public DateTime? LastClose { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
} 