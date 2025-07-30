using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using SharedService.Domain.Events;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Entities;

public class Currency : AggregateRoot<CurrencyCode>
{
    public CurrencyName Name { get; private set; }
    public CurrencySymbol Symbol { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private Currency() : base(CurrencyCode.From("XXX"))
    {
        Name = new CurrencyName("Temp");
        Symbol = new CurrencySymbol("$");
    }

    public Currency(CurrencyCode code, CurrencyName name, CurrencySymbol symbol) : base(code)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CurrencyCreatedEvent(Id, Name, Symbol));
    }

    public void UpdateName(CurrencyName newName)
    {
        if (newName == null)
            throw new ArgumentNullException(nameof(newName));

        var oldName = Name;
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CurrencyNameUpdatedEvent(Id, oldName, newName));
    }

    public void UpdateSymbol(CurrencySymbol newSymbol)
    {
        if (newSymbol == null)
            throw new ArgumentNullException(nameof(newSymbol));

        var oldSymbol = Symbol;
        Symbol = newSymbol;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new CurrencySymbolUpdatedEvent(Id, oldSymbol, newSymbol));
    }
} 