using BuildingBlocks.Domain.Entities;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Entities;

public class StockMovement : Entity<StockMovementId>
{
    public ProductId ProductId { get; private set; }
    public StoreId StoreId { get; private set; }
    public int QuantityChange { get; private set; }
    public MovementTypeValue MovementType { get; private set; }
    public DateTime MovementDate { get; private set; }
    public EmployeeId EmployeeId { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public EmployeeId? CreatedBy { get; private set; }

    private StockMovement() : base(StockMovementId.New())
    {
        ProductId = ProductId.New();
        StoreId = StoreId.From(1);
        MovementType = MovementTypeValue.Adjustment;
        EmployeeId = EmployeeId.New();
    }

    public StockMovement(
        StockMovementId id,
        ProductId productId,
        StoreId storeId,
        int quantityChange,
        MovementTypeValue movementType,
        EmployeeId employeeId,
        DateTime movementDate,
        Guid? referenceId = null,
        EmployeeId? createdBy = null) : base(id)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        QuantityChange = quantityChange;
        MovementType = movementType ?? throw new ArgumentNullException(nameof(movementType));
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        MovementDate = movementDate;
        ReferenceId = referenceId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public static StockMovement CreatePurchase(
        ProductId productId,
        StoreId storeId,
        int quantity,
        EmployeeId employeeId,
        Guid? purchaseOrderId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Purchase quantity must be positive", nameof(quantity));

        return new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            quantity,
            MovementTypeValue.Purchase,
            employeeId,
            DateTime.UtcNow,
            purchaseOrderId,
            employeeId);
    }

    public static StockMovement CreateReturn(
        ProductId productId,
        StoreId storeId,
        int quantity,
        EmployeeId employeeId,
        Guid? returnId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Return quantity must be positive", nameof(quantity));

        return new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            quantity,
            MovementTypeValue.Return,
            employeeId,
            DateTime.UtcNow,
            returnId,
            employeeId);
    }

    public static StockMovement CreateAdjustment(
        ProductId productId,
        StoreId storeId,
        int quantityChange,
        EmployeeId employeeId,
        Guid? adjustmentId = null)
    {
        return new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            quantityChange,
            MovementTypeValue.Adjustment,
            employeeId,
            DateTime.UtcNow,
            adjustmentId,
            employeeId);
    }

    public static StockMovement CreateDamage(
        ProductId productId,
        StoreId storeId,
        int quantity,
        EmployeeId employeeId,
        Guid? damageReportId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Damage quantity must be positive", nameof(quantity));

        return new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            -quantity,
            MovementTypeValue.Damage,
            employeeId,
            DateTime.UtcNow,
            damageReportId,
            employeeId);
    }

    public static StockMovement CreateTransfer(
        ProductId productId,
        StoreId storeId,
        int quantity,
        EmployeeId employeeId,
        bool isOutbound,
        Guid? transferId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Transfer quantity must be positive", nameof(quantity));

        var actualQuantity = isOutbound ? -quantity : quantity;

        return new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            actualQuantity,
            MovementTypeValue.Transfer,
            employeeId,
            DateTime.UtcNow,
            transferId,
            employeeId);
    }
}