namespace SupplierService.Application.DTOs;

public class PurchaseOrderDetailDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public int? ReceivedQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }

    public decimal LineTotal { get; set; }
    public int PendingQuantity { get; set; }
    public bool IsFullyReceived { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
} 