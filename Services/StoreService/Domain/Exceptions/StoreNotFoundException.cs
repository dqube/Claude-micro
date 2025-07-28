using BuildingBlocks.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Exceptions;

public class StoreNotFoundException : DomainException
{
    public StoreNotFoundException(StoreId storeId) 
        : base($"Store with ID '{storeId.Value}' was not found.")
    {
        StoreId = storeId;
    }

    public StoreNotFoundException(string storeName) 
        : base($"Store with name '{storeName}' was not found.")
    {
        StoreName = storeName;
    }

    public StoreId? StoreId { get; }
    public string? StoreName { get; }
} 