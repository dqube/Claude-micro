namespace BuildingBlocks.Application.Messaging;

public class MessageMetadata
{
    public MessageMetadata()
    {
        Headers = new Dictionary<string, string>();
        Properties = new Dictionary<string, object>();
        Timestamp = DateTime.UtcNow;
        MessageId = Guid.NewGuid().ToString();
    }

    public string MessageId { get; set; }
    public string? CorrelationId { get; set; }
    public string? CausationId { get; set; }
    public string? UserId { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? MessageType { get; set; }
    public string? Source { get; set; }
    public string? Destination { get; set; }
    public int? Priority { get; set; }
    public TimeSpan? TimeToLive { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, object> Properties { get; set; }

    public void AddHeader(string key, string value)
    {
        Headers[key] = value;
    }

    public void AddProperty(string key, object value)
    {
        Properties[key] = value;
    }

    public string? GetHeader(string key)
    {
        return Headers.TryGetValue(key, out var value) ? value : null;
    }

    public T? GetProperty<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }
}