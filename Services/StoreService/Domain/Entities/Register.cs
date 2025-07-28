using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using StoreService.Domain.Events;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Entities;

public class Register : AggregateRoot<RegisterId>
{
    public StoreId StoreId { get; private set; }
    public string Name { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public RegisterStatus Status { get; private set; }
    public DateTime? LastOpen { get; private set; }
    public DateTime? LastClose { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private Register() : base(RegisterId.From(1))
    {
        StoreId = StoreId.From(1);
        Name = string.Empty;
        Status = RegisterStatus.Closed;
    }

    public Register(
        RegisterId id,
        StoreId storeId,
        string name) : base(id)
    {
        ValidateName(name);

        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        Name = name.Trim();
        CurrentBalance = 0m;
        Status = RegisterStatus.Closed;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new RegisterCreatedEvent(Id, StoreId, Name));
    }

    public void Open(decimal startingCash)
    {
        if (Status.Equals(RegisterStatus.Open))
            throw new InvalidOperationException("Register is already open");

        if (startingCash < 0)
            throw new ArgumentException("Starting cash cannot be negative", nameof(startingCash));

        CurrentBalance = startingCash;
        Status = RegisterStatus.Open;
        LastOpen = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RegisterOpenedEvent(Id, StoreId, startingCash));
    }

    public void Close(decimal endingCash)
    {
        if (Status.Equals(RegisterStatus.Closed))
            throw new InvalidOperationException("Register is already closed");

        if (endingCash < 0)
            throw new ArgumentException("Ending cash cannot be negative", nameof(endingCash));

        var variance = endingCash - CurrentBalance;
        CurrentBalance = endingCash;
        Status = RegisterStatus.Closed;
        LastClose = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RegisterClosedEvent(Id, StoreId, endingCash, variance));
    }

    public void AddCash(decimal amount, string note = "")
    {
        ValidateAmount(amount);

        CurrentBalance += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CashAddedEvent(Id, amount, note));
    }

    public void RemoveCash(decimal amount, string note = "")
    {
        ValidateAmount(amount);

        if (amount > CurrentBalance)
            throw new InvalidOperationException("Cannot remove more cash than current balance");

        CurrentBalance -= amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CashRemovedEvent(Id, amount, note));
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);

        if (Name == newName.Trim()) return;

        var oldName = Name;
        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new RegisterNameUpdatedEvent(Id, oldName, Name));
    }

    public bool IsOpen => Status.Equals(RegisterStatus.Open);
    public bool IsClosed => Status.Equals(RegisterStatus.Closed);

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Register name cannot be null or empty", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("Register name cannot exceed 50 characters", nameof(name));
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));
    }
} 