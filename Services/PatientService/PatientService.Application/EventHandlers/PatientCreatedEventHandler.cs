using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using PatientService.Domain.Events;

namespace PatientService.Application.EventHandlers;

public class PatientCreatedEventHandler : IEventHandler<DomainEventWrapper<PatientCreatedEvent>>
{
    private readonly ILogger<PatientCreatedEventHandler> _logger;

    public PatientCreatedEventHandler(ILogger<PatientCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<PatientCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var patientCreatedEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Patient created: {PatientId} - {PatientName} (MRN: {MedicalRecordNumber})",
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
}