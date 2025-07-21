using BuildingBlocks.Domain.ValueObjects;

namespace PatientService.Domain.ValueObjects;

public class BloodType : Enumeration
{
    public static readonly BloodType OPositive = new(1, "O+");
    public static readonly BloodType ONegative = new(2, "O-");
    public static readonly BloodType APositive = new(3, "A+");
    public static readonly BloodType ANegative = new(4, "A-");
    public static readonly BloodType BPositive = new(5, "B+");
    public static readonly BloodType BNegative = new(6, "B-");
    public static readonly BloodType ABPositive = new(7, "AB+");
    public static readonly BloodType ABNegative = new(8, "AB-");
    public static readonly BloodType Unknown = new(9, "Unknown");

    private BloodType(int id, string name) : base(id, name) { }

    public static BloodType FromName(string name)
    {
        var bloodType = GetAll<BloodType>().SingleOrDefault(bt => 
            string.Equals(bt.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return bloodType ?? throw new ArgumentException($"Invalid blood type: {name}", nameof(name));
    }

    public static BloodType FromId(int id)
    {
        var bloodType = GetAll<BloodType>().SingleOrDefault(bt => bt.Id == id);
        return bloodType ?? throw new ArgumentException($"Invalid blood type ID: {id}", nameof(id));
    }
}