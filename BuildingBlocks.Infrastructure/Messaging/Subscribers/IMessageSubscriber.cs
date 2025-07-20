using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Infrastructure.Messaging.Subscribers;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : IMessage;
    Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : IMessage;
    Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : IMessage;
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
}