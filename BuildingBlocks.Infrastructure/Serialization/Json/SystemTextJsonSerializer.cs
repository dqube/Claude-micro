using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Serialization.Json;

public partial class SystemTextJsonSerializer : IJsonSerializer
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
            LogSerializeError(_logger, ex, typeof(T).Name);
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
            LogDeserializeError(_logger, ex, typeof(T).Name);
            throw;
        }
    }

    public object? Deserialize(string json, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        
        try
        {
            return JsonSerializer.Deserialize(json, type, _options);
        }
        catch (Exception ex)
        {
            LogDeserializeError(_logger, ex, type.Name);
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
            LogSerializeToBytesError(_logger, ex, typeof(T).Name);
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
            LogDeserializeFromBytesError(_logger, ex, typeof(T).Name);
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
            LogAsyncSerializeError(_logger, ex, typeof(T).Name);
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
            LogAsyncDeserializeError(_logger, ex, typeof(T).Name);
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

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error serializing object of type {typeName}")]
    private static partial void LogSerializeError(ILogger logger, Exception exception, string typeName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Error deserializing JSON to type {typeName}")]
    private static partial void LogDeserializeError(ILogger logger, Exception exception, string typeName);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Error serializing object to bytes for type {typeName}")]
    private static partial void LogSerializeToBytesError(ILogger logger, Exception exception, string typeName);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Error deserializing bytes to type {typeName}")]
    private static partial void LogDeserializeFromBytesError(ILogger logger, Exception exception, string typeName);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Error,
        Message = "Error async serializing object of type {typeName}")]
    private static partial void LogAsyncSerializeError(ILogger logger, Exception exception, string typeName);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Error,
        Message = "Error async deserializing JSON to type {typeName}")]
    private static partial void LogAsyncDeserializeError(ILogger logger, Exception exception, string typeName);
}