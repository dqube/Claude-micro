using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Events;
using CatalogService.Domain.BusinessRules;

namespace CatalogService.Domain.Entities;

public class Product : AggregateRoot<ProductId>
{
    public SKU SKU { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public Price BasePrice { get; private set; }
    public Price CostPrice { get; private set; }
    public bool IsTaxable { get; private set; }
    public bool IsDiscontinued { get; private set; }
    public DateTime? DiscontinuedAt { get; private set; }
    public bool IsInventoryTracked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private readonly List<ProductBarcode> _barcodes = new();
    public IReadOnlyCollection<ProductBarcode> Barcodes => _barcodes.AsReadOnly();

    private readonly List<CountryPricing> _countryPricing = new();
    public IReadOnlyCollection<CountryPricing> CountryPricing => _countryPricing.AsReadOnly();

    private static void CheckBusinessRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        
        if (rule.IsBroken())
        {
            throw new BuildingBlocks.Domain.Exceptions.BusinessRuleValidationException(rule);
        }
    }

    private Product() : base(ProductId.New())
    {
        SKU = SKU.From("TEMP");
        Name = string.Empty;
        CategoryId = CategoryId.From(1);
        BasePrice = Price.Zero;
        CostPrice = Price.Zero;
    }

    public Product(
        ProductId id,
        SKU sku,
        string name,
        CategoryId categoryId,
        Price basePrice,
        Price costPrice,
        string? description = null,
        bool isTaxable = true,
        Guid? createdBy = null) : base(id)
    {
        // Enforce business rules
        CheckBusinessRule(new ProductMustHaveValidSKURule(sku));
        CheckBusinessRule(new ProductMustHaveValidNameRule(name));
        CheckBusinessRule(new ProductMustHaveValidCategoryRule(categoryId));
        CheckBusinessRule(new ProductPriceMustBePositiveRule(basePrice, "Base price"));
        CheckBusinessRule(new ProductPriceMustBePositiveRule(costPrice, "Cost price"));
        CheckBusinessRule(new ProductCostPriceMustNotExceedBasePriceRule(costPrice, basePrice));

        SKU = sku;
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CategoryId = categoryId;
        BasePrice = basePrice;
        CostPrice = costPrice;
        IsTaxable = isTaxable;
        IsDiscontinued = false;
        IsInventoryTracked = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;

        AddDomainEvent(new ProductCreatedEvent(
            Id, SKU, Name, Description, CategoryId, BasePrice, CostPrice, IsTaxable, CreatedAt));
    }

    public void UpdateBasicInfo(string name, string? description, Guid updatedBy)
    {
        // Enforce business rules
        CheckBusinessRule(new ProductMustHaveValidNameRule(name));

        var oldName = Name;
        var oldDescription = Description;

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductBasicInfoUpdatedEvent(Id, oldName, Name, oldDescription, Description));
    }

    public void UpdatePricing(Price basePrice, Price costPrice, Guid updatedBy)
    {
        // Enforce business rules
        CheckBusinessRule(new ProductPriceMustBePositiveRule(basePrice, "Base price"));
        CheckBusinessRule(new ProductPriceMustBePositiveRule(costPrice, "Cost price"));
        CheckBusinessRule(new ProductCostPriceMustNotExceedBasePriceRule(costPrice, basePrice));

        var oldBasePrice = BasePrice;
        var oldCostPrice = CostPrice;
        
        BasePrice = basePrice;
        CostPrice = costPrice;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductPricingUpdatedEvent(Id, oldBasePrice, BasePrice, oldCostPrice, CostPrice));
    }

    public void UpdateTaxability(bool isTaxable, Guid updatedBy)
    {
        if (IsTaxable == isTaxable) return;

        IsTaxable = isTaxable;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductTaxabilityUpdatedEvent(Id, SKU, Name, IsTaxable));
    }

    public void MoveToCategory(CategoryId newCategoryId, Guid updatedBy)
    {
        // Enforce business rules
        CheckBusinessRule(new ProductMustHaveValidCategoryRule(newCategoryId));

        if (CategoryId.Equals(newCategoryId)) return;

        var oldCategoryId = CategoryId;
        CategoryId = newCategoryId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        AddDomainEvent(new ProductCategoryMovedEvent(Id, SKU, Name, oldCategoryId, newCategoryId, DateTime.UtcNow));
    }

    public void Discontinue(string? reason = null, Guid? discontinuedBy = null)
    {
        if (IsDiscontinued) return;

        IsDiscontinued = true;
        DiscontinuedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = discontinuedBy;

        AddDomainEvent(new ProductDiscontinuedEvent(Id, SKU, Name, CategoryId, DiscontinuedAt.Value, reason));
    }

    public void Reactivate(string? reason = null, Guid? reactivatedBy = null)
    {
        if (!IsDiscontinued) return;

        IsDiscontinued = false;
        var reactivatedAt = DateTime.UtcNow;
        DiscontinuedAt = null;
        UpdatedAt = reactivatedAt;
        UpdatedBy = reactivatedBy;

        AddDomainEvent(new ProductReactivatedEvent(Id, SKU, Name, CategoryId, reactivatedAt, reason));
    }

    public void EnableInventoryTracking(int initialQuantity = 0, Guid? enabledBy = null)
    {
        if (IsInventoryTracked) return;

        IsInventoryTracked = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = enabledBy;

        AddDomainEvent(new ProductInventoryTrackingEnabledEvent(Id, SKU, Name, DateTime.UtcNow, initialQuantity));
    }

    public void DisableInventoryTracking(int? finalQuantity = null, Guid? disabledBy = null)
    {
        if (!IsInventoryTracked) return;

        IsInventoryTracked = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;

        AddDomainEvent(new ProductInventoryTrackingDisabledEvent(Id, SKU, Name, DateTime.UtcNow, finalQuantity));
    }

    public void AddBarcode(BarcodeId barcodeId, BarcodeValue barcodeValue, BarcodeType barcodeType, Guid? addedBy = null)
    {
        // Enforce business rules
        var existingBarcodes = _barcodes.Select(b => b.BarcodeValue);
        CheckBusinessRule(new BarcodeMustBeUniqueRule(barcodeValue, existingBarcodes));

        var barcode = new ProductBarcode(barcodeId, Id, barcodeValue, barcodeType);
        _barcodes.Add(barcode);
        
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = addedBy;

        AddDomainEvent(new ProductBarcodeAddedEvent(Id, barcodeId, barcodeValue, barcodeType));
    }

    public void RemoveBarcode(BarcodeId barcodeId, Guid? removedBy = null)
    {
        var barcode = _barcodes.FirstOrDefault(b => b.Id.Equals(barcodeId));
        if (barcode == null) return;

        _barcodes.Remove(barcode);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = removedBy;

        AddDomainEvent(new ProductBarcodeRemovedEvent(Id, barcodeId, barcode.BarcodeValue));
    }

    public void AddCountryPricing(PricingId pricingId, CountryCode countryCode, Price price, DateTime effectiveDate, Guid? addedBy = null)
    {
        // Enforce business rules
        CheckBusinessRule(new CountryPricingMustHaveValidCountryRule(countryCode));
        CheckBusinessRule(new ProductPriceMustBePositiveRule(price, "Country price"));

        // Remove existing pricing for same country if it exists
        var existingPricing = _countryPricing.FirstOrDefault(cp => cp.CountryCode.Equals(countryCode));
        if (existingPricing != null)
        {
            _countryPricing.Remove(existingPricing);
        }

        var countryPricing = new CountryPricing(pricingId, Id, countryCode, price, effectiveDate);
        _countryPricing.Add(countryPricing);
        
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = addedBy;

        AddDomainEvent(new ProductCountryPricingUpdatedEvent(Id, countryCode, price, effectiveDate));
    }

    public void RemoveCountryPricing(CountryCode countryCode, Guid? removedBy = null)
    {
        var countryPricing = _countryPricing.FirstOrDefault(cp => cp.CountryCode.Equals(countryCode));
        if (countryPricing == null) return;

        _countryPricing.Remove(countryPricing);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = removedBy;

        // Could add a ProductCountryPricingRemovedEvent if needed
    }

    public bool CanBeDeleted()
    {
        // Business rule: Products with active inventory tracking or recent sales cannot be deleted
        return IsDiscontinued && !IsInventoryTracked;
    }

    public Price GetPriceForCountry(CountryCode countryCode)
    {
        var countryPricing = _countryPricing
            .Where(cp => cp.CountryCode.Equals(countryCode) && cp.EffectiveDate <= DateTime.UtcNow)
            .OrderByDescending(cp => cp.EffectiveDate)
            .FirstOrDefault();

        return countryPricing?.Price ?? BasePrice;
    }
}