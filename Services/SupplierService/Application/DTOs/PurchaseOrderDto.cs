namespace SupplierService.Application.DTOs;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public int StoreId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Guid? ShippingAddressId { get; set; }
    public Guid? ContactPersonId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public string SupplierName { get; set; } = string.Empty;
    public SupplierAddressDto? ShippingAddress { get; set; }
    public SupplierContactDto? ContactPerson { get; set; }
    public List<PurchaseOrderDetailDto> OrderDetails { get; set; } = new();
} 