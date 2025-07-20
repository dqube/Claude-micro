namespace BuildingBlocks.Infrastructure.Messaging.Serialization;

public interface IMessageSerializer
{
    string Serialize<T>(T message);
    T? Deserialize<T>(string serializedMessage);
    object? Deserialize(string serializedMessage, Type type);
    byte[] SerializeToBytes<T>(T message);
    T? DeserializeFromBytes<T>(byte[] serializedMessage);
}