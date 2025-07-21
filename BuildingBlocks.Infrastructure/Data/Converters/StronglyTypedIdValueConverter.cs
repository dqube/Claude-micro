using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace BuildingBlocks.Infrastructure.Data.Converters;

/// <summary>
/// EF Core value converter for strongly typed IDs that converts between the strongly typed ID and its underlying value
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly typed ID type</typeparam>
/// <typeparam name="TValue">The underlying value type (e.g., Guid, int, string)</typeparam>
public class StronglyTypedIdValueConverter<TStronglyTypedId, TValue> : ValueConverter<TStronglyTypedId, TValue>
    where TStronglyTypedId : StronglyTypedId<TValue>
    where TValue : notnull
{
    public StronglyTypedIdValueConverter() : base(
        convertToProviderExpression: id => id.Value,
        convertFromProviderExpression: value => CreateStronglyTypedId(value))
    {
    }

    /// <summary>
    /// Creates a strongly typed ID instance from the underlying value
    /// Uses reflection to find and invoke the appropriate constructor
    /// </summary>
    private static TStronglyTypedId CreateStronglyTypedId(TValue value)
    {
        // Try to find a constructor that takes the value type
        var constructor = typeof(TStronglyTypedId).GetConstructor(new[] { typeof(TValue) });
        if (constructor != null)
        {
            return (TStronglyTypedId)constructor.Invoke(new object[] { value });
        }

        // Try to find a static factory method (common pattern: From, Create, etc.)
        var fromMethod = typeof(TStronglyTypedId).GetMethod("From", new[] { typeof(TValue) });
        if (fromMethod?.IsStatic == true)
        {
            return (TStronglyTypedId)fromMethod.Invoke(null, new object[] { value })!;
        }

        var createMethod = typeof(TStronglyTypedId).GetMethod("Create", new[] { typeof(TValue) });
        if (createMethod?.IsStatic == true)
        {
            return (TStronglyTypedId)createMethod.Invoke(null, new object[] { value })!;
        }

        // Fallback: try Activator.CreateInstance
        try
        {
            return (TStronglyTypedId)Activator.CreateInstance(typeof(TStronglyTypedId), value)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not create instance of {typeof(TStronglyTypedId).Name} from value {value}. " +
                $"Ensure the type has a constructor that accepts {typeof(TValue).Name} or a static From/Create method.", ex);
        }
    }
}

/// <summary>
/// Specialized value converter for Guid-based strongly typed IDs
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly typed ID type that inherits from GuidId</typeparam>
public class GuidStronglyTypedIdValueConverter<TStronglyTypedId> : StronglyTypedIdValueConverter<TStronglyTypedId, Guid>
    where TStronglyTypedId : GuidId
{
}

/// <summary>
/// Specialized value converter for int-based strongly typed IDs
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly typed ID type that inherits from StronglyTypedId&lt;int&gt;</typeparam>
public class IntStronglyTypedIdValueConverter<TStronglyTypedId> : StronglyTypedIdValueConverter<TStronglyTypedId, int>
    where TStronglyTypedId : StronglyTypedId<int>
{
}

/// <summary>
/// Specialized value converter for string-based strongly typed IDs
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly typed ID type that inherits from StronglyTypedId&lt;string&gt;</typeparam>
public class StringStronglyTypedIdValueConverter<TStronglyTypedId> : StronglyTypedIdValueConverter<TStronglyTypedId, string>
    where TStronglyTypedId : StronglyTypedId<string>
{
}