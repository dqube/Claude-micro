using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Mediator;

public interface IMediator
{
    // Command dispatching
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TResult>;

    // Query dispatching
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;

    // Event dispatching
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    Task PublishAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    // Message dispatching
    Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;

    Task DispatchAsync<TMessage>(TMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;

    Task DispatchAsync(object message, CancellationToken cancellationToken = default);
    
    Task DispatchAsync(object message, MessageMetadata metadata, CancellationToken cancellationToken = default);
}