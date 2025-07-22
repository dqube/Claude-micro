using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Infrastructure.Data.Converters;

/// <summary>
/// Value converter selector that automatically provides value converters for strongly typed IDs
/// </summary>
public class StronglyTypedIdValueConverterSelector : ValueConverterSelector
{
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters = new();

    public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
    {
        var baseConverters = base.Select(modelClrType, providerClrType);
        
        // Check if this is a strongly typed ID
        if (IsStronglyTypedId(modelClrType))
        {
            var stronglyTypedIdConverter = GetStronglyTypedIdConverter(modelClrType, providerClrType);
            if (stronglyTypedIdConverter != null)
            {
                var converters = baseConverters.ToList();
                converters.Add(stronglyTypedIdConverter.Value);
                return converters;
            }
        }

        return baseConverters;
    }

    private static bool IsStronglyTypedId(Type type)
    {
        if (!typeof(IStronglyTypedId).IsAssignableFrom(type))
            return false;

        if (type.IsAbstract)
            return false;

        // Check if it inherits from StronglyTypedId<T>
        var baseType = type.BaseType;
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

    private ValueConverterInfo? GetStronglyTypedIdConverter(Type stronglyTypedIdType, Type? providerClrType)
    {
        var stronglyTypedIdBaseType = GetStronglyTypedIdBaseType(stronglyTypedIdType);
        if (stronglyTypedIdBaseType == null)
            return null;

        var valueType = stronglyTypedIdBaseType.GetGenericArguments()[0];
        
        // If provider type is specified and doesn't match the value type, return null
        if (providerClrType != null && providerClrType != valueType)
            return null;

        var key = (stronglyTypedIdType, valueType);
        
        return _converters.GetOrAdd(key, _ =>
        {
            var converterType = typeof(StronglyTypedIdValueConverter<,>).MakeGenericType(stronglyTypedIdType, valueType);
            return new ValueConverterInfo(
                modelClrType: stronglyTypedIdType,
                providerClrType: valueType,
                factory: _ => (ValueConverter)Activator.CreateInstance(converterType)!
            );
        });
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
/// Extension methods for configuring strongly typed ID value converters
/// </summary>
public static class StronglyTypedIdValueConverterExtensions
{
    /// <summary>
    /// Configures the DbContext to use strongly typed ID value converters
    /// Call this in OnConfiguring or when configuring DbContextOptions
    /// </summary>
    public static void UseStronglyTypedIdConverters(this DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();
    }

    /// <summary>
    /// Manually configures a strongly typed ID property in entity configuration
    /// Use this if you need specific control over the conversion
    /// </summary>
    public static PropertyBuilder<TProperty> HasStronglyTypedIdConversion<TProperty, TValue>(
        this PropertyBuilder<TProperty> propertyBuilder)
        where TProperty : StronglyTypedId<TValue>
        where TValue : notnull
    {
        return propertyBuilder.HasConversion<StronglyTypedIdValueConverter<TProperty, TValue>>();
    }

    /// <summary>
    /// Configures all strongly typed ID properties in a model automatically
    /// Call this in OnModelCreating
    /// </summary>
    public static void ConfigureStronglyTypedIds(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var propertyType = property.ClrType;
                if (IsStronglyTypedId(propertyType))
                {
                    var valueType = GetStronglyTypedIdValueType(propertyType);
                    if (valueType != null)
                    {
                        var converterType = typeof(StronglyTypedIdValueConverter<,>)
                            .MakeGenericType(propertyType, valueType);
                        
                        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }

    private static bool IsStronglyTypedId(Type type)
    {
        if (!typeof(IStronglyTypedId).IsAssignableFrom(type))
            return false;

        var baseType = type.BaseType;
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

    private static Type? GetStronglyTypedIdValueType(Type stronglyTypedIdType)
    {
        var baseType = stronglyTypedIdType.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>))
            {
                return baseType.GetGenericArguments()[0];
            }
            baseType = baseType.BaseType;
        }
        return null;
    }
}