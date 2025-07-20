namespace BuildingBlocks.Application.CQRS.Events;

public interface IIntegrationEvent : IEvent
{
    string CorrelationId { get; }
    string Source { get; }
    string Version { get; }
}