using BuildingBlocks.Domain.Entities;
using PromotionService.Domain.Events;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Entities;

public class DiscountCampaign : AggregateRoot<CampaignId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public int? MaxUsesPerCustomer { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private readonly List<DiscountRule> _rules = new();
    public IReadOnlyCollection<DiscountRule> Rules => _rules.AsReadOnly();

    // Private constructor for EF Core
    private DiscountCampaign() : base(CampaignId.New())
    {
        Name = string.Empty;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(1);
    }

    public DiscountCampaign(
        CampaignId id,
        string name,
        DateTime startDate,
        DateTime endDate,
        string? description = null,
        int? maxUsesPerCustomer = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign name cannot be null or empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Campaign name cannot exceed 100 characters", nameof(name));

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        MaxUsesPerCustomer = maxUsesPerCustomer;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new DiscountCampaignCreatedEvent(Id, Name, StartDate, EndDate));
    }

    public void UpdateDetails(string name, string? description, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign name cannot be null or empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Campaign name cannot exceed 100 characters", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new DiscountCampaignUpdatedEvent(Id, Name));
    }

    public void UpdateDates(DateTime startDate, DateTime endDate, Guid updatedBy)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(Guid activatedBy)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;

        AddDomainEvent(new DiscountCampaignActivatedEvent(Id));
    }

    public void Deactivate(Guid deactivatedBy)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;

        AddDomainEvent(new DiscountCampaignDeactivatedEvent(Id));
    }

    public void AddRule(DiscountRule rule)
    {
        _rules.Add(rule);
    }

    public void RemoveRule(RuleId ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule != null)
        {
            _rules.Remove(rule);
        }
    }

    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        return IsActive && now >= StartDate && now <= EndDate;
    }
} 