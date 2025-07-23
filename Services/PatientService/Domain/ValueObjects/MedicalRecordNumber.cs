using BuildingBlocks.Domain.ValueObjects;

namespace PatientService.Domain.ValueObjects;

public class MedicalRecordNumber : ValueObject
{
    public string Value { get; }

    public MedicalRecordNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Medical record number cannot be null or empty", nameof(value));
        
        if (value.Length < 5 || value.Length > 20)
            throw new ArgumentException("Medical record number must be between 5 and 20 characters", nameof(value));

        Value = value.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(MedicalRecordNumber mrn)
    {
        ArgumentNullException.ThrowIfNull(mrn);
        return mrn.Value;
    }
    public static explicit operator MedicalRecordNumber(string value) => new(value);
}