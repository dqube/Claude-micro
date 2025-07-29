using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Events;
using CatalogService.Domain.BusinessRules;

namespace CatalogService.Domain.Entities;

public class ProductCategory : AggregateRoot<CategoryId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public CategoryId? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
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

    private ProductCategory() : base(CategoryId.From(1))
    {
        Name = string.Empty;
    }

    public ProductCategory(
        CategoryId id,
        string name,
        string? description = null,
        CategoryId? parentCategoryId = null,
        Guid? createdBy = null,
        IEnumerable<string>? existingCategoryNames = null) : base(id)
    {
        // Enforce business rules
        CheckBusinessRule(new CategoryMustHaveValidNameRule(name));
        CheckBusinessRule(new CategoryCannotBeItsOwnParentRule(id, parentCategoryId));
        
        if (existingCategoryNames != null)
        {
            CheckBusinessRule(new CategoryNameMustBeUniqueRule(name, existingCategoryNames));
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ParentCategoryId = parentCategoryId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        // Determine parent category name for rich domain events
        string? parentCategoryName = null; // In real implementation, this would be passed in or resolved
        
        AddDomainEvent(new ProductCategoryCreatedEvent(
            Id, Name, Description, ParentCategoryId, parentCategoryName, CreatedAt));
    }

    public void UpdateName(string name, Guid updatedBy, IEnumerable<string>? existingCategoryNames = null)
    {
        // Enforce business rules
        CheckBusinessRule(new CategoryMustHaveValidNameRule(name));
        
        if (existingCategoryNames != null)
        {
            CheckBusinessRule(new CategoryNameMustBeUniqueRule(name, existingCategoryNames));
        }

        if (Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)) return;

        var oldName = Name;
        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductCategoryNameUpdatedEvent(Id, oldName, Name));
    }

    public void UpdateDescription(string? description, Guid updatedBy)
    {
        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        
        if (Description == trimmedDescription) return;

        Description = trimmedDescription;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Could add a ProductCategoryDescriptionUpdatedEvent if needed
    }

    public void UpdateParentCategory(CategoryId? parentCategoryId, Guid updatedBy, string? parentCategoryName = null)
    {
        // Enforce business rules
        CheckBusinessRule(new CategoryCannotBeItsOwnParentRule(Id, parentCategoryId));

        if ((ParentCategoryId == null && parentCategoryId == null) || 
            (ParentCategoryId != null && ParentCategoryId.Equals(parentCategoryId))) 
            return;

        var oldParentId = ParentCategoryId;
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductCategoryParentUpdatedEvent(Id, oldParentId, ParentCategoryId));
    }

    public void Deactivate(Guid deactivatedBy)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;

        // Could add a ProductCategoryDeactivatedEvent
    }

    public void Activate(Guid activatedBy)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;

        // Could add a ProductCategoryActivatedEvent
    }

    public bool CanBeDeleted(bool hasProducts = false, bool hasChildCategories = false)
    {
        // Business rule: Categories with products or child categories cannot be deleted
        return !hasProducts && !hasChildCategories;
    }

    public bool IsRootCategory()
    {
        return ParentCategoryId == null;
    }

    public void PrepareForDeletion(
        int productsMovedCount = 0, 
        CategoryId? productsMovedToCategoryId = null,
        int childCategoriesMovedCount = 0,
        Guid? deletedBy = null)
    {
        // Emit rich domain event before deletion
        string? parentCategoryName = null; // In real implementation, this would be resolved
        
        AddDomainEvent(new ProductCategoryDeletedEvent(
            Id, 
            Name, 
            ParentCategoryId, 
            parentCategoryName,
            DateTime.UtcNow,
            productsMovedCount,
            productsMovedToCategoryId,
            childCategoriesMovedCount));

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
}