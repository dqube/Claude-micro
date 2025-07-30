namespace SupplierService.Application.DTOs;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxIdentificationNumber { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<SupplierContactDto> Contacts { get; set; } = new();
    public List<SupplierAddressDto> Addresses { get; set; } = new();
    public List<PurchaseOrderDto> PurchaseOrders { get; set; } = new();
} 