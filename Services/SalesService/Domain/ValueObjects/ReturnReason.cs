using BuildingBlocks.Domain.ValueObjects;

namespace SalesService.Domain.ValueObjects;

public class ReturnReason : Enumeration
{
    public static readonly ReturnReason Defective = new(1, "Defective");
    public static readonly ReturnReason WrongItem = new(2, "WrongItem");
    public static readonly ReturnReason CustomerChange = new(3, "CustomerChange");
    public static readonly ReturnReason Other = new(4, "Other");

    public ReturnReason(int id, string name) : base(id, name)
    {
    }

    public static ReturnReason FromName(string name)
    {
        return GetAll<ReturnReason>().FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"Invalid return reason: {name}");
    }

    public static ReturnReason FromId(int id)
    {
        return GetAll<ReturnReason>().FirstOrDefault(r => r.Id == id)
               ?? throw new ArgumentException($"Invalid return reason id: {id}");
    }
}