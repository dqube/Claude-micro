using BuildingBlocks.Domain.ValueObjects;

namespace StoreService.Domain.ValueObjects;

public class MovementType : Enumeration
{
    public static readonly MovementType Open = new(1, "Open");
    public static readonly MovementType Close = new(2, "Close");
    public static readonly MovementType CashIn = new(3, "CashIn");
    public static readonly MovementType CashOut = new(4, "CashOut");

    private MovementType(int id, string name) : base(id, name) { }

    public static MovementType FromName(string name)
    {
        var type = GetAll<MovementType>().SingleOrDefault(t => 
            string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return type ?? throw new ArgumentException($"Invalid movement type: {name}", nameof(name));
    }

    public static MovementType FromId(int id)
    {
        var type = GetAll<MovementType>().SingleOrDefault(t => t.Id == id);
        return type ?? throw new ArgumentException($"Invalid movement type ID: {id}", nameof(id));
    }
} 