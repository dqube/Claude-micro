using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds.Json;

/// <summary>
/// JSON converter for strongly typed IDs that serializes/deserializes the underlying value
/// </summary>
/// <typeparam name="TStronglyTypedId">The strongly typed ID type</typeparam>
/// <typeparam name="TValue">The underlying value type (e.g., Guid, int, string)</typeparam>
public class StronglyTypedIdJsonConverter<TStronglyTypedId, TValue> : JsonConverter<TStronglyTypedId>
    where TStronglyTypedId : StronglyTypedId<TValue>
    where TValue : notnull
{
    public override TStronglyTypedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null!;
        }

        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value == null)
        {
            return null!;
        }

        // Create instance using reflection since we can't use generic constraints with constructors
        return (TStronglyTypedId)Activator.CreateInstance(typeToConvert, value)!;
    }

    public override void Write(Utf8JsonWriter writer, TStronglyTypedId value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(TStronglyTypedId).IsAssignableFrom(typeToConvert);
    }
}