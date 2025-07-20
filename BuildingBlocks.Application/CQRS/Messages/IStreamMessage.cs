namespace BuildingBlocks.Application.CQRS.Messages;

public interface IStreamMessage : IMessage
{
    string StreamName { get; }
    long ExpectedVersion { get; }
}