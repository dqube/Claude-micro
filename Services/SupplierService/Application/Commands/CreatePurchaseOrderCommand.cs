using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class CreatePurchaseOrderCommand : CommandBase<PurchaseOrderDto>
{
    public Guid SupplierId { get; init; }
    public int StoreId { get; init; }
    public DateTime? ExpectedDate { get; init; }
    public Guid? ShippingAddressId { get; init; }
    public Guid? ContactPersonId { get; init; }
    public List<CreatePurchaseOrderDetailDto> OrderDetails { get; init; } = new();

    public CreatePurchaseOrderCommand(Guid supplierId, int storeId, DateTime? expectedDate = null, Guid? shippingAddressId = null, Guid? contactPersonId = null)
    {
        SupplierId = supplierId;
        StoreId = storeId;
        ExpectedDate = expectedDate;
        ShippingAddressId = shippingAddressId;
        ContactPersonId = contactPersonId;
    }
}

public class CreatePurchaseOrderDetailDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
} 