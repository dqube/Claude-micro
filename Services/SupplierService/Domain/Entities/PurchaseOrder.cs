using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;
using SupplierService.Domain.Events;

namespace SupplierService.Domain.Entities;

public class PurchaseOrder : AggregateRoot<OrderId>
{
    private readonly List<PurchaseOrderDetail> _orderDetails = new();

    public SupplierId SupplierId { get; private set; } = null!;
    public int StoreId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; private set; }
    public AddressId? ShippingAddressId { get; private set; }
    public ContactId? ContactPersonId { get; private set; }

    // Audit properties
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyList<PurchaseOrderDetail> OrderDetails => _orderDetails.AsReadOnly();

    private PurchaseOrder() { } // EF Core

    public PurchaseOrder(
        OrderId id,
        SupplierId supplierId,
        int storeId,
        DateTime? orderDate = null,
        DateTime? expectedDate = null,
        AddressId? shippingAddressId = null,
        ContactId? contactPersonId = null,
        Guid? createdBy = null) : base(id)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        StoreId = storeId;
        OrderDate = orderDate ?? DateTime.UtcNow;
        ExpectedDate = expectedDate;
        Status = PurchaseOrderStatus.Draft;
        TotalAmount = 0m;
        ShippingAddressId = shippingAddressId;
        ContactPersonId = contactPersonId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;

        AddDomainEvent(new PurchaseOrderCreatedEvent(Id, SupplierId, StoreId, OrderDate));
    }

    public void UpdateBasicInfo(
        DateTime? expectedDate,
        AddressId? shippingAddressId,
        ContactId? contactPersonId,
        Guid updatedBy)
    {
        ExpectedDate = expectedDate;
        ShippingAddressId = shippingAddressId;
        ContactPersonId = contactPersonId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new PurchaseOrderUpdatedEvent(Id, SupplierId, Status.Value));
    }

    public void AddOrderDetail(PurchaseOrderDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);
        
        _orderDetails.Add(detail);
        RecalculateTotal();
        
        AddDomainEvent(new PurchaseOrderDetailAddedEvent(Id, detail.Id, detail.ProductId, detail.Quantity, detail.UnitCost));
    }

    public void RemoveOrderDetail(OrderDetailId detailId)
    {
        var detail = _orderDetails.FirstOrDefault(d => d.Id == detailId);
        if (detail == null) return;

        _orderDetails.Remove(detail);
        RecalculateTotal();
        
        AddDomainEvent(new PurchaseOrderDetailRemovedEvent(Id, detailId, detail.ProductId));
    }

    public void UpdateOrderDetail(OrderDetailId detailId, int quantity, decimal unitCost, Guid updatedBy)
    {
        var detail = _orderDetails.FirstOrDefault(d => d.Id == detailId);
        if (detail == null)
            throw new InvalidOperationException($"Order detail with ID {detailId} not found");

        detail.UpdateQuantityAndCost(quantity, unitCost, updatedBy);
        RecalculateTotal();
        
        AddDomainEvent(new PurchaseOrderDetailUpdatedEvent(Id, detailId, detail.ProductId, quantity, unitCost));
    }

    public void SubmitOrder(Guid updatedBy)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted");

        if (_orderDetails.Count == 0)
            throw new InvalidOperationException("Cannot submit order without order details");

        Status = PurchaseOrderStatus.Ordered;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        
        AddDomainEvent(new PurchaseOrderSubmittedEvent(Id, SupplierId, TotalAmount));
    }

    public void ReceiveOrder(Guid updatedBy)
    {
        if (Status != PurchaseOrderStatus.Ordered)
            throw new InvalidOperationException("Only ordered purchase orders can be received");

        Status = PurchaseOrderStatus.Received;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        
        AddDomainEvent(new PurchaseOrderReceivedEvent(Id, SupplierId, TotalAmount));
    }

    public void CancelOrder(Guid updatedBy)
    {
        if (Status == PurchaseOrderStatus.Received)
            throw new InvalidOperationException("Cannot cancel received orders");

        Status = PurchaseOrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        
        AddDomainEvent(new PurchaseOrderCancelledEvent(Id, SupplierId, Status.Value));
    }

    public void ReceivePartialQuantity(OrderDetailId detailId, int receivedQuantity, Guid updatedBy)
    {
        var detail = _orderDetails.FirstOrDefault(d => d.Id == detailId);
        if (detail == null)
            throw new InvalidOperationException($"Order detail with ID {detailId} not found");

        detail.ReceiveQuantity(receivedQuantity, updatedBy);
        
        AddDomainEvent(new PurchaseOrderPartiallyReceivedEvent(Id, detailId, detail.ProductId, receivedQuantity));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _orderDetails.Sum(d => d.LineTotal);
    }
} 