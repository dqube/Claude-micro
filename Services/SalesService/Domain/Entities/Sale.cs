using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.BusinessRules;
using BuildingBlocks.Domain.Exceptions;
using SalesService.Domain.Events;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.BusinessRules;

namespace SalesService.Domain.Entities;

public class Sale : AggregateRoot<SaleId>, IAuditableEntity, ISoftDeletable
{
    private readonly List<SaleDetail> _saleDetails = new();
    private readonly List<AppliedDiscount> _appliedDiscounts = new();

    public int StoreId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public int RegisterId { get; private set; }
    public DateTime TransactionTime { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ReceiptNumber ReceiptNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public IReadOnlyCollection<SaleDetail> SaleDetails => _saleDetails.AsReadOnly();
    public IReadOnlyCollection<AppliedDiscount> AppliedDiscounts => _appliedDiscounts.AsReadOnly();

    private Sale() : base(SaleId.New())
    {
        ReceiptNumber = ReceiptNumber.From("temp");
    }

    public Sale(
        SaleId id,
        int storeId,
        Guid employeeId,
        int registerId,
        ReceiptNumber receiptNumber,
        Guid? customerId = null) : base(id)
    {
        StoreId = storeId;
        EmployeeId = employeeId;
        CustomerId = customerId;
        RegisterId = registerId;
        ReceiptNumber = receiptNumber;
        TransactionTime = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        SubTotal = 0;
        DiscountTotal = 0;
        TaxAmount = 0;
        TotalAmount = 0;

        AddDomainEvent(new SaleCreatedEvent(Id, StoreId, EmployeeId, CustomerId));
    }

    public void AddSaleDetail(Guid productId, int quantity, decimal unitPrice, decimal taxApplied = 0)
    {
        CheckRule(new QuantityMustBePositiveRule(quantity));
        CheckRule(new PriceMustBePositiveRule(unitPrice));
        CheckRule(new TaxRateMustBeValidRule(taxApplied));

        var saleDetail = new SaleDetail(
            SaleDetailId.New(),
            Id,
            productId,
            quantity,
            unitPrice,
            taxApplied);
        
        _saleDetails.Add(saleDetail);
        RecalculateTotals();

        AddDomainEvent(new SaleDetailAddedEvent(Id, saleDetail.Id, productId, quantity, unitPrice));
    }

    public void ApplyDiscount(Guid? saleDetailId, Guid campaignId, Guid ruleId, decimal discountAmount)
    {
        CheckRule(new DiscountAmountMustBePositiveRule(discountAmount));

        var appliedDiscount = new AppliedDiscount(
            AppliedDiscountId.New(),
            saleDetailId != null ? SaleDetailId.From(saleDetailId.Value) : null,
            saleDetailId == null ? Id : null,
            campaignId,
            ruleId,
            discountAmount);
        
        _appliedDiscounts.Add(appliedDiscount);
        RecalculateTotals();

        AddDomainEvent(new DiscountAppliedEvent(Id, campaignId, ruleId, discountAmount));
    }

    public void CompleteSale(Guid? completedBy = null)
    {
        CheckRule(new SaleCannotBeEmptyRule(_saleDetails.Count));

        CreatedBy = completedBy?.ToString();
        RecalculateTotals();

        AddDomainEvent(new SaleCompletedEvent(Id, TotalAmount, TransactionTime));
    }

    private void RecalculateTotals()
    {
        SubTotal = _saleDetails.Sum(d => d.LineTotal);
        DiscountTotal = _appliedDiscounts.Sum(d => d.DiscountAmount);
        TaxAmount = _saleDetails.Sum(d => d.TaxApplied);
        TotalAmount = SubTotal - DiscountTotal + TaxAmount;

        if (TotalAmount < 0)
            TotalAmount = 0;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    private static void CheckRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}