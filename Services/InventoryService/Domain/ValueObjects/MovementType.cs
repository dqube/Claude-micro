namespace InventoryService.Domain.ValueObjects;

public enum MovementType
{
    Purchase,
    Return,
    Adjustment,
    Damage,
    Transfer
}

public class MovementTypeValue
{
    public MovementType Value { get; private set; }

    private MovementTypeValue(MovementType value)
    {
        Value = value;
    }

    public static MovementTypeValue Purchase => new(MovementType.Purchase);
    public static MovementTypeValue Return => new(MovementType.Return);
    public static MovementTypeValue Adjustment => new(MovementType.Adjustment);
    public static MovementTypeValue Damage => new(MovementType.Damage);
    public static MovementTypeValue Transfer => new(MovementType.Transfer);

    public static MovementTypeValue From(string value)
    {
        return value?.ToUpperInvariant() switch
        {
            "PURCHASE" => Purchase,
            "RETURN" => Return,
            "ADJUSTMENT" => Adjustment,
            "DAMAGE" => Damage,
            "TRANSFER" => Transfer,
            _ => throw new ArgumentException($"Invalid movement type: {value}", nameof(value))
        };
    }

    public static MovementTypeValue From(MovementType value)
    {
        return new MovementTypeValue(value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator string(MovementTypeValue movementType) => movementType.ToString();
    public static implicit operator MovementType(MovementTypeValue movementType) => movementType.Value;
}