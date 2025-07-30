namespace SupplierService.Application.DTOs;

public class SupplierAddressDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public int AddressTypeId { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsShipping { get; set; }
    public bool IsBilling { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public string FullAddress { get; set; } = string.Empty;
} 