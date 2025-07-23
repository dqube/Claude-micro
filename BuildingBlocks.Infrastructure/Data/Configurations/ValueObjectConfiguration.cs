using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

public static class ValueObjectConfiguration
{
    public static PropertyBuilder<TValueObject> ConfigureValueObject<TValueObject>(
        this PropertyBuilder<TValueObject> propertyBuilder)
        where TValueObject : ValueObject
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);
        
        return propertyBuilder
            .HasConversion(
                vo => vo.ToString(),
                value => CreateValueObject<TValueObject>(value!))
            .IsRequired();
    }

    public static PropertyBuilder<TValueObject?> ConfigureNullableValueObject<TValueObject>(
        this PropertyBuilder<TValueObject?> propertyBuilder)
        where TValueObject : ValueObject
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);
        
        return propertyBuilder
            .HasConversion(
#pragma warning disable CS8604, CS8625 // Expression tree limitations with nullable reference types
                vo => vo != null ? vo.ToString() : null,
                value => value != null ? CreateValueObject<TValueObject>(value) : null);
#pragma warning restore CS8604, CS8625
    }

    private static TValueObject CreateValueObject<TValueObject>(string value)
        where TValueObject : ValueObject
    {
        ArgumentNullException.ThrowIfNull(value);
        
        var constructor = typeof(TValueObject).GetConstructor(new[] { typeof(string) });
        
        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Value object {typeof(TValueObject).Name} must have a constructor that accepts a string parameter");
        }

        return (TValueObject)constructor.Invoke(new object[] { value });
    }
}