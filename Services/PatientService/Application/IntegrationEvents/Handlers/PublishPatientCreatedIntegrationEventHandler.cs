using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Domain.DomainEvents;
using Microsoft.Extensions.Logging;
using PatientService.Domain.Entities;
using PatientService.Domain.Events;
using PatientService.Application.IntegrationEvents;
using BuildingBlocks.Domain.Repository;

namespace PatientService.Application.IntegrationEvents.Handlers;

/// <summary>
/// Publishes integration events to Kafka when a patient is created
/// </summary>
public class PublishPatientCreatedIntegrationEventHandler : IDomainEventHandler<PatientCreatedEvent>
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<PublishPatientCreatedIntegrationEventHandler> _logger;
    private readonly IReadOnlyRepository<Patient, PatientService.Domain.ValueObjects.PatientId> _patientRepository;

    public PublishPatientCreatedIntegrationEventHandler(
        IMessageBus messageBus,
        ILogger<PublishPatientCreatedIntegrationEventHandler> logger,
        IReadOnlyRepository<Patient, PatientService.Domain.ValueObjects.PatientId> patientRepository)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }

    public async Task HandleAsync(PatientCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Publishing integration event for patient created: {PatientId}",
            domainEvent.PatientId);

        // Get full patient details from repository
        var patient = await _patientRepository.GetByIdAsync(domainEvent.PatientId, cancellationToken);
        
        if (patient == null)
        {
            _logger.LogWarning("Patient {PatientId} not found for integration event", domainEvent.PatientId);
            return;
        }

        var integrationEvent = new PatientCreatedIntegrationEvent(
            domainEvent.PatientId.Value,
            domainEvent.MedicalRecordNumber.Value,
            domainEvent.Name.FirstName,
            domainEvent.Name.LastName,
            patient.DateOfBirth,
            patient.Gender.ToString(),
            patient.Email.Value,
            patient.PhoneNumber?.Value,
            patient.CreatedAt
        );

        // Publish to Kafka topic "patient.created"
        await _messageBus.PublishAsync(integrationEvent, "patient.created", cancellationToken);

        _logger.LogInformation(
            "Successfully published integration event for patient: {PatientId} to topic 'patient.created'",
            domainEvent.PatientId);
    }
}
