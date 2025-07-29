using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Events;
using CatalogService.Domain.BusinessRules;

namespace CatalogService.Domain.Entities;

public class TaxConfiguration : AggregateRoot<TaxConfigId>
{
    public int LocationId { get; private set; }
    public CategoryId? CategoryId { get; private set; }
    public TaxRate TaxRate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private static void CheckBusinessRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        
        if (rule.IsBroken())
        {
            throw new BuildingBlocks.Domain.Exceptions.BusinessRuleValidationException(rule);
        }
    }

    private TaxConfiguration() : base(TaxConfigId.New())
    {
        LocationId = 1;
        TaxRate = TaxRate.Zero;
    }

    public TaxConfiguration(
        TaxConfigId id,
        int locationId,
        TaxRate taxRate,
        CategoryId? categoryId = null,
        DateTime? effectiveDate = null,
        DateTime? expiryDate = null,
        Guid? createdBy = null,
        IEnumerable<(int LocationId, CategoryId? CategoryId)>? existingConfigurations = null) : base(id)
    {
        // Enforce business rules
        CheckBusinessRule(new TaxConfigurationMustHaveValidLocationRule(locationId));
        CheckBusinessRule(new TaxRateMustBeValidRule(taxRate));
        
        if (existingConfigurations != null)
        {
            CheckBusinessRule(new TaxConfigurationMustBeUniqueRule(locationId, categoryId, existingConfigurations));
        }

        LocationId = locationId;
        CategoryId = categoryId;
        TaxRate = taxRate;
        IsActive = true;
        EffectiveDate = effectiveDate ?? DateTime.UtcNow;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        AddDomainEvent(new TaxConfigurationCreatedEvent(Id, LocationId, CategoryId, TaxRate));
    }

    public void UpdateTaxRate(TaxRate newTaxRate, Guid updatedBy)
    {
        // Enforce business rules
        CheckBusinessRule(new TaxRateMustBeValidRule(newTaxRate));

        if (TaxRate.Equals(newTaxRate)) return;

        var oldTaxRate = TaxRate;
        TaxRate = newTaxRate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new TaxConfigurationUpdatedEvent(Id, LocationId, CategoryId, oldTaxRate, TaxRate));
    }

    public void UpdateCategory(CategoryId? categoryId, Guid updatedBy, 
        IEnumerable<(int LocationId, CategoryId? CategoryId)>? existingConfigurations = null)
    {
        if (existingConfigurations != null)
        {
            CheckBusinessRule(new TaxConfigurationMustBeUniqueRule(LocationId, categoryId, existingConfigurations));
        }

        if ((CategoryId == null && categoryId == null) || 
            (CategoryId != null && CategoryId.Equals(categoryId))) 
            return;

        var oldCategoryId = CategoryId;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new TaxConfigurationCategoryUpdatedEvent(Id, LocationId, oldCategoryId, CategoryId));
    }

    public void UpdateEffectivePeriod(DateTime effectiveDate, DateTime? expiryDate, Guid updatedBy)
    {
        if (expiryDate.HasValue && effectiveDate >= expiryDate.Value)
            throw new ArgumentException("Effective date must be before expiry date");

        EffectiveDate = effectiveDate;
        ExpiryDate = expiryDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Could add a TaxConfigurationEffectivePeriodUpdatedEvent
    }

    public void Activate(Guid activatedBy)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;

        // Could add a TaxConfigurationActivatedEvent
    }

    public void Deactivate(Guid deactivatedBy)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;

        // Could add a TaxConfigurationDeactivatedEvent
    }

    public bool AppliesTo(CategoryId? productCategoryId, DateTime? dateToCheck = null)
    {
        var checkDate = dateToCheck ?? DateTime.UtcNow;
        
        // Check if configuration is active and within effective period
        if (!IsActive || checkDate < EffectiveDate || (ExpiryDate.HasValue && checkDate > ExpiryDate.Value))
            return false;

        // Check category applicability - null category means applies to all categories
        return CategoryId == null || CategoryId.Equals(productCategoryId);
    }

    public bool IsExpired(DateTime? dateToCheck = null)
    {
        var checkDate = dateToCheck ?? DateTime.UtcNow;
        return ExpiryDate.HasValue && checkDate > ExpiryDate.Value;
    }

    public bool IsEffective(DateTime? dateToCheck = null)
    {
        var checkDate = dateToCheck ?? DateTime.UtcNow;
        return IsActive && checkDate >= EffectiveDate && (!ExpiryDate.HasValue || checkDate <= ExpiryDate.Value);
    }

    public int GetPriorityScore()
    {
        // Category-specific configurations have higher priority than general ones
        return CategoryId == null ? 1 : 2;
    }
}