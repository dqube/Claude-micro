namespace BuildingBlocks.Application.Inbox;

public interface IInboxMessageHandler
{
    Task HandleAsync(object message, CancellationToken cancellationToken = default);
    bool CanHandle(string messageType);
}

public interface IInboxMessageHandler<in TMessage> : IInboxMessageHandler
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    
    async Task IInboxMessageHandler.HandleAsync(object message, CancellationToken cancellationToken)
    {
        if (message is TMessage typedMessage)
        {
            await HandleAsync(typedMessage, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Cannot handle message of type {message?.GetType().Name}");
        }
    }
}