using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Converters;

/// <summary>
/// Custom JSON converter for Guid objects that handles various string formats
/// </summary>
public class CustomGuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        if (string.IsNullOrEmpty(value))
            throw new JsonException("Guid value cannot be null or empty");

        if (Guid.TryParse(value, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to parse '{value}' as Guid");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("D")); // Standard format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
    }
}