namespace BuildingBlocks.Application.CQRS.Messages;

public abstract class MessageBase : IMessage
{
    protected MessageBase()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        MessageType = GetType().Name;
    }

    public Guid Id { get; }
    public DateTime Timestamp { get; }
    public string MessageType { get; }
}