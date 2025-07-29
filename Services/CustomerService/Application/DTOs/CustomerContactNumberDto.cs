using BuildingBlocks.Application.DTOs;

namespace CustomerService.Application.DTOs;

public class CustomerContactNumberDto : BaseDto
{
    public new Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public int ContactNumberTypeId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool Verified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
} 