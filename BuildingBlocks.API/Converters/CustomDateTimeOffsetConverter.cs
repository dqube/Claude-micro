using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Converters;

/// <summary>
/// Custom JSON converter for DateTimeOffset objects with multiple format support
/// </summary>
public class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private readonly string[] _formats = {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss.fffffffZ",
        "yyyy-MM-ddTHH:mm:sszzz",
        "yyyy-MM-ddTHH:mm:ss.fffzzz",
        "yyyy-MM-ddTHH:mm:ss.fffffffzzz"
    };

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        if (string.IsNullOrEmpty(value))
            throw new JsonException("DateTimeOffset value cannot be null or empty");

        // Try parsing with each format
        foreach (var format in _formats)
        {
            if (DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        // Fallback to default parsing
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallbackResult))
        {
            return fallbackResult;
        }

        throw new JsonException($"Unable to parse '{value}' as DateTimeOffset. Supported formats: {string.Join(", ", _formats)}");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
    }
}