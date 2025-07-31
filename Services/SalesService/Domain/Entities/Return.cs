using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.BusinessRules;
using BuildingBlocks.Domain.Exceptions;
using SalesService.Domain.Events;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.BusinessRules;

namespace SalesService.Domain.Entities;

public class Return : AggregateRoot<ReturnId>, IAuditableEntity, ISoftDeletable
{
    private readonly List<ReturnDetail> _returnDetails = new();

    public SaleId SaleId { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public decimal TotalRefund { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public IReadOnlyCollection<ReturnDetail> ReturnDetails => _returnDetails.AsReadOnly();

    private Return() : base(ReturnId.New())
    {
        SaleId = SaleId.New();
    }

    public Return(
        ReturnId id,
        SaleId saleId,
        Guid employeeId,
        Guid? customerId = null) : base(id)
    {
        SaleId = saleId ?? throw new ArgumentNullException(nameof(saleId));
        EmployeeId = employeeId;
        CustomerId = customerId;
        ReturnDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        TotalRefund = 0;

        AddDomainEvent(new ReturnCreatedEvent(Id, SaleId, EmployeeId, CustomerId));
    }

    public void AddReturnDetail(Guid productId, int quantity, ReturnReason reason, bool restock = true)
    {
        CheckRule(new QuantityMustBePositiveRule(quantity));

        var returnDetail = new ReturnDetail(
            ReturnDetailId.New(),
            Id,
            productId,
            quantity,
            reason,
            restock);
        
        _returnDetails.Add(returnDetail);

        AddDomainEvent(new ReturnDetailAddedEvent(Id, productId, quantity, reason, restock));
    }

    public void ProcessReturn(decimal totalRefund, Guid? processedBy = null)
    {
        CheckRule(new ReturnCannotBeEmptyRule(_returnDetails.Count));
        CheckRule(new RefundAmountMustBePositiveRule(totalRefund));

        TotalRefund = totalRefund;
        CreatedBy = processedBy?.ToString();

        AddDomainEvent(new ReturnProcessedEvent(Id, SaleId, TotalRefund));
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