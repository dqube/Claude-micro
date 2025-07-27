using BuildingBlocks.Domain.ValueObjects;

namespace ContactService.Domain.ValueObjects;

public class ContactType : Enumeration
{
    public static readonly ContactType Personal = new(1, nameof(Personal));
    public static readonly ContactType Business = new(2, nameof(Business));
    public static readonly ContactType Emergency = new(3, nameof(Emergency));

    public ContactType(int id, string name) : base(id, name)
    {
    }

    public static ContactType From(string name)
    {
        return GetAll<ContactType>().FirstOrDefault(ct => ct.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Invalid contact type: {name}");
    }

    public static ContactType From(int id)
    {
        return GetAll<ContactType>().FirstOrDefault(ct => ct.Id == id)
            ?? throw new ArgumentException($"Invalid contact type id: {id}");
    }
}