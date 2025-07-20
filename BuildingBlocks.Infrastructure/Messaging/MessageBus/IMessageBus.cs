using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.MessageBus;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(T message, string destination, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(T message, MessageMetadata metadata, CancellationToken cancellationToken = default) where T : class;
    Task SubscribeAsync<T>(Func<T, MessageMetadata, Task> handler, CancellationToken cancellationToken = default) where T : class;
    Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class;
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}