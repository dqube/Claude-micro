namespace BuildingBlocks.Infrastructure.Serialization.Xml;

public interface IXmlSerializer
{
    string Serialize<T>(T obj);
    T? Deserialize<T>(string xml);
    object? Deserialize(string xml, Type type);
    byte[] SerializeToBytes<T>(T obj);
    T? DeserializeFromBytes<T>(byte[] xmlBytes);
}