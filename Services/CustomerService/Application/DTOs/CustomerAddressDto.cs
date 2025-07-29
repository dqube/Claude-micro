using BuildingBlocks.Application.DTOs;

namespace CustomerService.Application.DTOs;

public class CustomerAddressDto : BaseDto
{
    public new Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public int AddressTypeId { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public string FormattedAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
} 