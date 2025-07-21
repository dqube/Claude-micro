using System.Text.Json;

namespace BuildingBlocks.Domain.StronglyTypedIds.Json.Examples;

/// <summary>
/// Example demonstrating how to use the StronglyTypedId JSON converters
/// </summary>
public static class JsonConverterUsageExample
{
    /// <summary>
    /// Example strongly typed ID for demonstration
    /// </summary>
    public class ExampleId : GuidId
    {
        public ExampleId(Guid value) : base(value) { }
        public ExampleId() : base() { }
        
        public static ExampleId New() => new();
        public static ExampleId From(Guid value) => new(value);
    }

    /// <summary>
    /// Example entity using strongly typed ID
    /// </summary>
    public record ExampleEntity(ExampleId Id, string Name, DateTime CreatedAt);

    /// <summary>
    /// Demonstrates serialization and deserialization of strongly typed IDs
    /// </summary>
    public static void DemonstrateUsage()
    {
        // Create JsonSerializerOptions with strongly typed ID support
        var options = StronglyTypedIdJsonExtensions.CreateWithStronglyTypedIdSupport();
        
        // Create example entity
        var entity = new ExampleEntity(
            ExampleId.New(),
            "Test Entity", 
            DateTime.UtcNow
        );
        
        // Serialize to JSON - the strongly typed ID will be serialized as its underlying Guid value
        var json = JsonSerializer.Serialize(entity, options);
        Console.WriteLine($"Serialized JSON: {json}");
        
        // Deserialize from JSON - the Guid value will be converted back to the strongly typed ID
        var deserializedEntity = JsonSerializer.Deserialize<ExampleEntity>(json, options);
        Console.WriteLine($"Deserialized Entity: {deserializedEntity}");
        
        // Verify they are equal
        var areEqual = entity.Id.Equals(deserializedEntity?.Id);
        Console.WriteLine($"IDs are equal: {areEqual}");
    }

    /// <summary>
    /// Shows how to manually configure JsonSerializerOptions
    /// </summary>
    public static JsonSerializerOptions GetConfiguredOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        // Add the strongly typed ID converter factory
        options.AddStronglyTypedIdConverters();
        
        return options;
    }

    /// <summary>
    /// Example of handling collections with strongly typed IDs
    /// </summary>
    public static void DemonstrateCollectionSerialization()
    {
        var options = StronglyTypedIdJsonExtensions.CreateWithStronglyTypedIdSupport();
        
        var entities = new List<ExampleEntity>
        {
            new(ExampleId.New(), "Entity 1", DateTime.UtcNow),
            new(ExampleId.New(), "Entity 2", DateTime.UtcNow),
            new(ExampleId.New(), "Entity 3", DateTime.UtcNow)
        };
        
        var json = JsonSerializer.Serialize(entities, options);
        Console.WriteLine($"Collection JSON: {json}");
        
        var deserializedEntities = JsonSerializer.Deserialize<List<ExampleEntity>>(json, options);
        Console.WriteLine($"Deserialized {deserializedEntities?.Count} entities");
    }
}