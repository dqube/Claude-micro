using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Dispatchers;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public MessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        var handlers = _serviceProvider.GetServices<IMessageHandler<TMessage>>();
        
        var tasks = handlers.Select(handler => handler.HandleAsync(message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task DispatchAsync<TMessage>(TMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        var handlers = _serviceProvider.GetServices<IMessageHandler<TMessage>>();
        
        var tasks = handlers.Select(handler => handler.HandleAsync(message, metadata, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        var handlers = _serviceProvider.GetServices<IMessageHandler>()
            .Where(h => h.CanHandle(message.GetType()));
        
        var tasks = handlers.Select(handler => handler.HandleAsync(message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task DispatchAsync(object message, MessageMetadata metadata, CancellationToken cancellationToken = default)
    {
        var handlers = _serviceProvider.GetServices<IMessageHandler>()
            .Where(h => h.CanHandle(message.GetType()));
        
        var tasks = handlers.Select(handler => handler.HandleAsync(message, metadata, cancellationToken));
        await Task.WhenAll(tasks);
    }
}