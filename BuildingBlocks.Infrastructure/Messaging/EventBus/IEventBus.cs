using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Infrastructure.Messaging.EventBus;

public interface IEventBus
{
    /// <summary>
    /// Publishes a single event asynchronously.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="evt">The event instance.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task PublishAsync<T>(T evt, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    /// Publishes multiple events asynchronously.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="events">The collection of events.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    /// Subscribes a handler to an event type.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="handler">The handler function.</param>
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler) where T : IEvent;

    /// <summary>
    /// Unsubscribes all handlers for an event type.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    Task UnsubscribeAsync<T>() where T : IEvent;
}