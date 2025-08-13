using BuildingBlocks.Domain.Entities;
using PaymentService.Domain.Events;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities;

public class PaymentProcessor : AggregateRoot<PaymentProcessorId>
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public decimal CommissionRate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private PaymentProcessor() : base(PaymentProcessorId.New())
    {
        Name = string.Empty;
        IsActive = true;
        CommissionRate = 0;
    }

    public PaymentProcessor(
        PaymentProcessorId id,
        string name,
        decimal commissionRate = 0,
        bool isActive = true) : base(id)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Processor name cannot be empty", nameof(name));
        CommissionRate = commissionRate >= 0 ? commissionRate : throw new ArgumentException("Commission rate cannot be negative", nameof(commissionRate));
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentProcessorCreatedEvent(Id, Name, CommissionRate, IsActive));
    }

    public void UpdateDetails(string name, decimal commissionRate, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Processor name cannot be empty", nameof(name));
        
        if (commissionRate < 0)
            throw new ArgumentException("Commission rate cannot be negative", nameof(commissionRate));

        var oldName = Name;
        var oldCommissionRate = CommissionRate;

        Name = name;
        CommissionRate = commissionRate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new PaymentProcessorUpdatedEvent(Id, oldName, Name, oldCommissionRate, CommissionRate));
    }

    public void Activate(Guid? updatedBy = null)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new PaymentProcessorActivatedEvent(Id, Name));
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new PaymentProcessorDeactivatedEvent(Id, Name));
    }
} 