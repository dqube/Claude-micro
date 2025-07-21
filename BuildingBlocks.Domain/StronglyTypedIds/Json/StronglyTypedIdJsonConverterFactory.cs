using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds.Json;

/// <summary>
/// JSON converter factory that automatically creates converters for strongly typed IDs
/// </summary>
public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if the type implements IStronglyTypedId
        if (!typeof(IStronglyTypedId).IsAssignableFrom(typeToConvert))
            return false;

        // Check if it's a concrete type (not abstract)
        if (typeToConvert.IsAbstract)
            return false;

        // Check if it inherits from StronglyTypedId<T>
        var baseType = typeToConvert.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        return false;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        // Find the StronglyTypedId<T> base type to get the underlying value type
        var stronglyTypedIdBaseType = GetStronglyTypedIdBaseType(typeToConvert);
        if (stronglyTypedIdBaseType == null)
        {
            throw new InvalidOperationException($"Type {typeToConvert.Name} does not inherit from StronglyTypedId<T>");
        }

        var valueType = stronglyTypedIdBaseType.GetGenericArguments()[0];

        // Create the generic converter type
        var converterType = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(typeToConvert, valueType);

        // Create and return the converter instance
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetStronglyTypedIdBaseType(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
            {
                return baseType;
            }
            baseType = baseType.BaseType;
        }
        return null;
    }
}

/// <summary>
/// Extension methods to register strongly typed ID JSON converters
/// </summary>
public static class StronglyTypedIdJsonExtensions
{
    /// <summary>
    /// Adds the strongly typed ID JSON converter factory to JsonSerializerOptions
    /// </summary>
    /// <param name="options">The JsonSerializerOptions to configure</param>
    /// <returns>The same JsonSerializerOptions instance for chaining</returns>
    public static JsonSerializerOptions AddStronglyTypedIdConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        return options;
    }

    /// <summary>
    /// Creates JsonSerializerOptions with strongly typed ID converters pre-configured
    /// </summary>
    /// <returns>JsonSerializerOptions with strongly typed ID support</returns>
    public static JsonSerializerOptions CreateWithStronglyTypedIdSupport()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        return options.AddStronglyTypedIdConverters();
    }
}