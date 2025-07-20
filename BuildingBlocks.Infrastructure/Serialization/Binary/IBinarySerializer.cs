namespace BuildingBlocks.Infrastructure.Serialization.Binary;

public interface IBinarySerializer
{
    byte[] Serialize<T>(T obj);
    T? Deserialize<T>(byte[] data);
    object? Deserialize(byte[] data, Type type);
    Stream SerializeToStream<T>(T obj);
    T? DeserializeFromStream<T>(Stream stream);
}