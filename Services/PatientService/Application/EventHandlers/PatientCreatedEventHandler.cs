using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using PatientService.Domain.Events;

namespace PatientService.Application.EventHandlers;

public partial class PatientCreatedEventHandler : IEventHandler<DomainEventWrapper<PatientCreatedEvent>>
{
    private readonly ILogger<PatientCreatedEventHandler> _logger;

    public PatientCreatedEventHandler(ILogger<PatientCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(DomainEventWrapper<PatientCreatedEvent> notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        var patientCreatedEvent = notification.DomainEvent;
        
        LogPatientCreated(
            _logger,
            patientCreatedEvent.PatientId.Value,
            patientCreatedEvent.Name.FullName,
            patientCreatedEvent.MedicalRecordNumber.Value);

        // Here you could add additional logic like:
        // - Send welcome email
        // - Create audit log entry
        // - Notify other services
        // - Send integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Patient created: {patientId} - {patientName} (MRN: {medicalRecordNumber})")]
    private static partial void LogPatientCreated(ILogger logger, Guid patientId, string patientName, string medicalRecordNumber);
}