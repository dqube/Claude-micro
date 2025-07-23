using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Converters;

/// <summary>
/// Custom JSON converter for enums that serializes as strings using camelCase naming policy
/// </summary>
public class CustomJsonStringEnumConverter : JsonConverterFactory
{
    private readonly JsonNamingPolicy? _namingPolicy;
    private readonly bool _allowIntegerValues;

    public CustomJsonStringEnumConverter() : this(JsonNamingPolicy.CamelCase, true)
    {
    }

    public CustomJsonStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    {
        _namingPolicy = namingPolicy;
        _allowIntegerValues = allowIntegerValues;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        return typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum ?? false);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        var isNullable = Nullable.GetUnderlyingType(typeToConvert) != null;
        
        if (isNullable)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(NullableEnumConverter<>).MakeGenericType(enumType),
                _namingPolicy,
                _allowIntegerValues)!;
        }
        else
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(EnumConverter<>).MakeGenericType(enumType),
                _namingPolicy,
                _allowIntegerValues)!;
        }
    }

    private sealed class EnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly JsonNamingPolicy? _namingPolicy;
        private readonly bool _allowIntegerValues;

        public EnumConverter(JsonNamingPolicy? namingPolicy, bool allowIntegerValues)
        {
            _namingPolicy = namingPolicy;
            _allowIntegerValues = allowIntegerValues;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    throw new JsonException($"Cannot convert empty string to enum {typeof(T)}");
                }

                // Try to parse as enum name (case-insensitive)
                if (Enum.TryParse<T>(stringValue, true, out var enumValue))
                {
                    return enumValue;
                }

                // Try camelCase conversion
                var pascalCase = char.ToUpperInvariant(stringValue[0]) + stringValue[1..];
                if (Enum.TryParse<T>(pascalCase, true, out var camelCaseValue))
                {
                    return camelCaseValue;
                }

                throw new JsonException($"Cannot convert '{stringValue}' to enum {typeof(T)}");
            }

            if (reader.TokenType == JsonTokenType.Number && _allowIntegerValues)
            {
                var intValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(T), intValue))
                {
                    return (T)Enum.ToObject(typeof(T), intValue);
                }
                throw new JsonException($"Cannot convert {intValue} to enum {typeof(T)}");
            }

            throw new JsonException($"Unexpected token type {reader.TokenType} for enum {typeof(T)}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var enumName = value.ToString();
            var convertedName = _namingPolicy?.ConvertName(enumName) ?? enumName;
            writer.WriteStringValue(convertedName);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via reflection in CreateConverter.")]
private sealed class NullableEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
    {
        private readonly EnumConverter<T> _enumConverter;

        public NullableEnumConverter(JsonNamingPolicy? namingPolicy, bool allowIntegerValues)
        {
            _enumConverter = new EnumConverter<T>(namingPolicy, allowIntegerValues);
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return _enumConverter.Read(ref reader, typeof(T), options);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            if (value.HasValue)
            {
                _enumConverter.Write(writer, value.Value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}