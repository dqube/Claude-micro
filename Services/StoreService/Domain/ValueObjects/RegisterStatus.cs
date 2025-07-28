using BuildingBlocks.Domain.ValueObjects;

namespace StoreService.Domain.ValueObjects;

public class RegisterStatus : Enumeration
{
    public static readonly RegisterStatus Open = new(1, "Open");
    public static readonly RegisterStatus Closed = new(2, "Closed");

    private RegisterStatus(int id, string name) : base(id, name) { }

    public static RegisterStatus FromName(string name)
    {
        var status = GetAll<RegisterStatus>().SingleOrDefault(s => 
            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return status ?? throw new ArgumentException($"Invalid register status: {name}", nameof(name));
    }

    public static RegisterStatus FromId(int id)
    {
        var status = GetAll<RegisterStatus>().SingleOrDefault(s => s.Id == id);
        return status ?? throw new ArgumentException($"Invalid register status ID: {id}", nameof(id));
    }
} 