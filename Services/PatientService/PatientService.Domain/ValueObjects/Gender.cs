using BuildingBlocks.Domain.ValueObjects;

namespace PatientService.Domain.ValueObjects;

public class Gender : Enumeration
{
    public static readonly Gender Male = new(1, "Male");
    public static readonly Gender Female = new(2, "Female");
    public static readonly Gender Other = new(3, "Other");
    public static readonly Gender PreferNotToSay = new(4, "Prefer not to say");

    private Gender(int id, string name) : base(id, name) { }

    public static Gender FromName(string name)
    {
        var gender = GetAll<Gender>().SingleOrDefault(g => 
            string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return gender ?? throw new ArgumentException($"Invalid gender: {name}", nameof(name));
    }

    public static Gender FromId(int id)
    {
        var gender = GetAll<Gender>().SingleOrDefault(g => g.Id == id);
        return gender ?? throw new ArgumentException($"Invalid gender ID: {id}", nameof(id));
    }
}