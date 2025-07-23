using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Serialization.Json;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<SystemTextJsonSerializer> _logger;

    public SystemTextJsonSerializer(
        JsonSerializerOptions? options = null,
        ILogger<SystemTextJsonSerializer>? logger = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<SystemTextJsonSerializer>.Instance;
    }

    public string Serialize<T>(T obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, _options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing object of type {Type}", typeof(T).Name);
            throw;
        }
    }

    public T? Deserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing JSON to type {Type}", typeof(T).Name);
            throw;
        }
    }

    public object? Deserialize(string json, Type type)
    {
        try
        {
            return JsonSerializer.Deserialize(json, type, _options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing JSON to type {Type}", type.Name);
            throw;
        }
    }

    public byte[] SerializeToBytes<T>(T obj)
    {
        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing object to bytes for type {Type}", typeof(T).Name);
            throw;
        }
    }

    public T? DeserializeFromBytes<T>(byte[] data)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(data, _options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing bytes to type {Type}", typeof(T).Name);
            throw;
        }
    }

    public async Task<string> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, obj, _options, cancellationToken);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error async serializing object of type {Type}", typeof(T).Name);
            throw;
        }
    }

    public async Task<T?> DeserializeAsync<T>(string json, CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            return await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error async deserializing JSON to type {Type}", typeof(T).Name);
            throw;
        }
    }

    public bool TryDeserialize<T>(string json, out T? result)
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(json, _options);
            return true;
        }
        catch (JsonException)
        {
            result = default;
            return false;
        }
        catch (ArgumentException)
        {
            result = default;
            return false;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public bool IsValidJson(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}