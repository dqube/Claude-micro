using BuildingBlocks.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Exceptions;

public class RegisterNotFoundException : DomainException
{
    public RegisterNotFoundException(RegisterId registerId) 
        : base($"Register with ID '{registerId.Value}' was not found.")
    {
        RegisterId = registerId;
    }

    public RegisterNotFoundException(string registerName, StoreId storeId) 
        : base($"Register with name '{registerName}' in store '{storeId.Value}' was not found.")
    {
        RegisterName = registerName;
        StoreId = storeId;
    }

    public RegisterId? RegisterId { get; }
    public string? RegisterName { get; }
    public StoreId? StoreId { get; }
} 