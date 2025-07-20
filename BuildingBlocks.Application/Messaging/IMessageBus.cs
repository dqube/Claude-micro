using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Application.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IMessage;
    Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : class, IMessage;
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class, IMessage;
    Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class, IMessage;
}