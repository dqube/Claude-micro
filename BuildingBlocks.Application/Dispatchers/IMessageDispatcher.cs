using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Dispatchers;

public interface IMessageDispatcher
{
    Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;

    Task DispatchAsync<TMessage>(TMessage message, MessageEnvelop metadata, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;

    Task DispatchAsync(object message, CancellationToken cancellationToken = default);
    
    Task DispatchAsync(object message, MessageEnvelop metadata, CancellationToken cancellationToken = default);
}