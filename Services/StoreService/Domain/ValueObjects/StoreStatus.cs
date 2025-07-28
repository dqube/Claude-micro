using BuildingBlocks.Domain.ValueObjects;

namespace StoreService.Domain.ValueObjects;

public class StoreStatus : Enumeration
{
    public static readonly StoreStatus Active = new(1, "Active");
    public static readonly StoreStatus Maintenance = new(2, "Maintenance");
    public static readonly StoreStatus Closed = new(3, "Closed");

    private StoreStatus(int id, string name) : base(id, name) { }

    public static StoreStatus FromName(string name)
    {
        var status = GetAll<StoreStatus>().SingleOrDefault(s => 
            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return status ?? throw new ArgumentException($"Invalid store status: {name}", nameof(name));
    }

    public static StoreStatus FromId(int id)
    {
        var status = GetAll<StoreStatus>().SingleOrDefault(s => s.Id == id);
        return status ?? throw new ArgumentException($"Invalid store status ID: {id}", nameof(id));
    }
} 