using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using SharedService.Domain.Events;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Entities;

public class Country : AggregateRoot<CountryCode>
{
    public CountryName Name { get; private set; }
    public CurrencyCode CurrencyCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Navigation property
    public Currency? Currency { get; private set; }

    // Private constructor for EF Core
    private Country() : base(CountryCode.From("XX"))
    {
        Name = new CountryName("Temp");
        CurrencyCode = SharedService.Domain.ValueObjects.CurrencyCode.From("XXX");
    }

    public Country(CountryCode code, CountryName name, CurrencyCode currencyCode) : base(code)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CurrencyCode = currencyCode ?? throw new ArgumentNullException(nameof(currencyCode));
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CountryCreatedEvent(Id, Name, CurrencyCode));
    }

    public void UpdateName(CountryName newName)
    {
        if (newName == null)
            throw new ArgumentNullException(nameof(newName));

        var oldName = Name;
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CountryNameUpdatedEvent(Id, oldName, newName));
    }

    public void UpdateCurrency(CurrencyCode newCurrencyCode)
    {
        if (newCurrencyCode == null)
            throw new ArgumentNullException(nameof(newCurrencyCode));

        var oldCurrencyCode = CurrencyCode;
        CurrencyCode = newCurrencyCode;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CountryCurrencyUpdatedEvent(Id, oldCurrencyCode, newCurrencyCode));
    }
} 