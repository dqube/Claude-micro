using BuildingBlocks.Domain.Entities;
using PromotionService.Domain.Events;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Entities;

public class Promotion : AggregateRoot<PromotionId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsCombinable { get; private set; }
    public int? MaxRedemptions { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private readonly List<PromotionProduct> _promotionProducts = new();
    public IReadOnlyCollection<PromotionProduct> PromotionProducts => _promotionProducts.AsReadOnly();

    // Private constructor for EF Core
    private Promotion() : base(PromotionId.New())
    {
        Name = string.Empty;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(1);
    }

    public Promotion(
        PromotionId id,
        string name,
        DateTime startDate,
        DateTime endDate,
        string? description = null,
        bool isCombinable = false,
        int? maxRedemptions = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Promotion name cannot be null or empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Promotion name cannot exceed 100 characters", nameof(name));

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsCombinable = isCombinable;
        MaxRedemptions = maxRedemptions;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new PromotionCreatedEvent(Id, Name, StartDate, EndDate));
    }

    public void UpdateDetails(string name, string? description, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Promotion name cannot be null or empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Promotion name cannot exceed 100 characters", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new PromotionUpdatedEvent(Id, Name));
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

    public void UpdateSettings(bool isCombinable, int? maxRedemptions, Guid updatedBy)
    {
        IsCombinable = isCombinable;
        MaxRedemptions = maxRedemptions;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void AddPromotionProduct(PromotionProduct promotionProduct)
    {
        _promotionProducts.Add(promotionProduct);
    }

    public void RemovePromotionProduct(PromotionProductId promotionProductId)
    {
        var promotionProduct = _promotionProducts.FirstOrDefault(pp => pp.Id == promotionProductId);
        if (promotionProduct != null)
        {
            _promotionProducts.Remove(promotionProduct);
        }
    }

    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }

    public bool CanBeCombined()
    {
        return IsCombinable;
    }
} 