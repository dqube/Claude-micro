using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Dispatchers;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Application.Mediator;

public class Mediator : IMediator
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IMessageDispatcher _messageDispatcher;

    public Mediator(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IEventDispatcher eventDispatcher,
        IMessageDispatcher messageDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _eventDispatcher = eventDispatcher;
        _messageDispatcher = messageDispatcher;
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        await _commandDispatcher.DispatchAsync(command, cancellationToken);
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TResult>
    {
        return await _commandDispatcher.DispatchAsync<TCommand, TResult>(command, cancellationToken);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        return await _queryDispatcher.DispatchAsync<TQuery, TResult>(query, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        await _eventDispatcher.DispatchAsync(@event, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        await _eventDispatcher.DispatchAsync(events, cancellationToken);
    }

    public async Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        await _messageDispatcher.DispatchAsync(message, cancellationToken);
    }

    public async Task DispatchAsync<TMessage>(TMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        await _messageDispatcher.DispatchAsync(message, metadata, cancellationToken);
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        await _messageDispatcher.DispatchAsync(message, cancellationToken);
    }

    public async Task DispatchAsync(object message, MessageMetadata metadata, CancellationToken cancellationToken = default)
    {
        await _messageDispatcher.DispatchAsync(message, metadata, cancellationToken);
    }
}