namespace BuildingBlocks.Application.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class;

    Task PublishAsync<TMessage>(TMessage message, string destination, CancellationToken cancellationToken = default)
        where TMessage : class;

    Task PublishAsync<TMessage>(TMessage message, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TMessage : class;

    Task PublishAsync<TMessage>(TMessage message, string destination, MessageMetadata metadata, CancellationToken cancellationToken = default)
        where TMessage : class;

    Task ScheduleAsync<TMessage>(TMessage message, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where TMessage : class;

    Task ScheduleAsync<TMessage>(TMessage message, string destination, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where TMessage : class;
}