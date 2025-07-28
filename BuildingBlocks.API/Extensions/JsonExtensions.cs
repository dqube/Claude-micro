using BuildingBlocks.API.Converters;
using BuildingBlocks.Domain.StronglyTypedIds.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for configuring JSON serialization options
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Configures HTTP JSON options with common converters and settings
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddApiJsonConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            // Configure JSON serialization options
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            
            // Add custom converters
            options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
            options.SerializerOptions.Converters.Add(new CustomNullableDateTimeConverter());
            options.SerializerOptions.Converters.Add(new CustomDateTimeOffsetConverter());
            options.SerializerOptions.Converters.Add(new CustomGuidConverter());
            options.SerializerOptions.Converters.Add(new CustomDecimalConverter());
            options.SerializerOptions.Converters.Add(new CustomJsonStringEnumConverter());
            
            // Add StronglyTypedId converter factory
            options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        });

        return services;
    }
    
    /// <summary>
    /// Gets the default JSON serializer options used by the API
    /// </summary>
    /// <returns>Configured JsonSerializerOptions</returns>
    public static JsonSerializerOptions GetApiJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
        
        // Add custom converters
        options.Converters.Add(new CustomDateTimeConverter());
        options.Converters.Add(new CustomNullableDateTimeConverter());
        options.Converters.Add(new CustomDateTimeOffsetConverter());
        options.Converters.Add(new CustomGuidConverter());
        options.Converters.Add(new CustomDecimalConverter());
        options.Converters.Add(new CustomJsonStringEnumConverter());
        
        // Add StronglyTypedId converter factory
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        
        return options;
    }
}