using System.Reflection;

namespace BuildingBlocks.Domain.ValueObjects;

public abstract class Enumeration : IComparable
{
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        ArgumentNullException.ThrowIfNull(displayName);
        var matchingItem = Parse<T, string>(displayName, "display name", item => string.Equals(item.Name, displayName, StringComparison.Ordinal));
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(predicate);
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

public int CompareTo(object? other)
{
    ArgumentNullException.ThrowIfNull(other);
    if (other is not Enumeration otherEnum)
        throw new ArgumentException($"Object must be of type {nameof(Enumeration)}", nameof(other));
    return Id.CompareTo(otherEnum.Id);
}
}