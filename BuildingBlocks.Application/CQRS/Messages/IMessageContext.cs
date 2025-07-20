namespace BuildingBlocks.Application.CQRS.Messages;

public interface IMessageContext
{
    string CorrelationId { get; }
    string? UserId { get; }
    DateTime Timestamp { get; }
    IDictionary<string, object> Properties { get; }
}