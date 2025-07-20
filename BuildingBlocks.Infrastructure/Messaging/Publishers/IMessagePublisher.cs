using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Infrastructure.Messaging.Publishers;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
    Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : IMessage;
    Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : IMessage;
}