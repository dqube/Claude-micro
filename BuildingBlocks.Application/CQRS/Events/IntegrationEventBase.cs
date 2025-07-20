namespace BuildingBlocks.Application.CQRS.Events;

public abstract class IntegrationEventBase : IIntegrationEvent
{
    protected IntegrationEventBase(string source, string correlationId = null!)
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
        Source = source;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Version = "1.0";
    }

    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    public string EventType { get; }
    public string CorrelationId { get; }
    public string Source { get; }
    public string Version { get; }
}