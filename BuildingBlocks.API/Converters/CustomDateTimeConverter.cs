using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Converters;

/// <summary>
/// Custom JSON converter for DateTime objects with multiple format support
/// </summary>
public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss.fffffffZ",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "M/d/yyyy",
        "d/M/yyyy"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            // Return default DateTime for null - validation will handle if this is invalid
            return default(DateTime);
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token type {reader.TokenType} for DateTime. Expected String.");
        }

        var value = reader.GetString();
        
        if (string.IsNullOrEmpty(value))
        {
            // Return default DateTime for empty string - validation will handle if this is invalid
            return default(DateTime);
        }

        // Try parsing with each format
        foreach (var format in _formats)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        // Fallback to default parsing
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallbackResult))
        {
            return fallbackResult;
        }

        throw new JsonException($"Unable to parse '{value}' as DateTime. Supported formats: {string.Join(", ", _formats)}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}