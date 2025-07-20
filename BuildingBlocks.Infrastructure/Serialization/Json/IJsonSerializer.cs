namespace BuildingBlocks.Infrastructure.Serialization.Json;

public interface IJsonSerializer
{
    string Serialize<T>(T obj);
    T? Deserialize<T>(string json);
    object? Deserialize(string json, Type type);
    byte[] SerializeToBytes<T>(T obj);
    T? DeserializeFromBytes<T>(byte[] data);
    Task<string> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default);
    Task<T?> DeserializeAsync<T>(string json, CancellationToken cancellationToken = default);
    bool TryDeserialize<T>(string json, out T? result);
    bool IsValidJson(string json);
}