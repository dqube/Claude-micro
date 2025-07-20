namespace BuildingBlocks.Application.CQRS.Messages;

public interface IMessage
{
    Guid Id { get; }
    DateTime Timestamp { get; }
  
}