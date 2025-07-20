namespace BuildingBlocks.Application.Messaging;

public interface IMessageHandler<in TMessage>
    where TMessage : class
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    Task HandleAsync(TMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default);
}

public interface IMessageHandler
{
    Task HandleAsync(object message, CancellationToken cancellationToken = default);
    Task HandleAsync(object message, MessageMetadata metadata, CancellationToken cancellationToken = default);
    bool CanHandle(Type messageType);
    bool CanHandle<T>() where T : class;
}