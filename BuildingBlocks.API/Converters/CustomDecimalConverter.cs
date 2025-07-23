using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Converters;

/// <summary>
/// Custom JSON converter for Decimal objects with consistent formatting
/// </summary>
public class CustomDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
                throw new JsonException("Decimal value cannot be null or empty");

            if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse '{stringValue}' as decimal");
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} for decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}