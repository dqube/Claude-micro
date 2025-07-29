using BuildingBlocks.Application.DTOs;

namespace CustomerService.Application.DTOs;

public class CustomerDto : BaseDto
{
    public new Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public int LoyaltyPoints { get; set; }
    public int? PreferredContactMethod { get; set; }
    public int? PreferredAddressType { get; set; }
    public bool IsMembershipActive { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public List<CustomerContactNumberDto> ContactNumbers { get; set; } = new();
    public List<CustomerAddressDto> Addresses { get; set; } = new();
} 