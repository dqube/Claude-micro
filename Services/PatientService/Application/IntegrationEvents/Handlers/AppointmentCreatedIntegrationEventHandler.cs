using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;
using PatientService.Application.IntegrationEvents;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using BuildingBlocks.Domain.Repository;

namespace PatientService.Application.IntegrationEvents.Handlers;

/// <summary>
/// Handles AppointmentCreatedIntegrationEvent from AppointmentService
/// Updates patient's appointment history when an appointment is created
/// </summary>
public class AppointmentCreatedIntegrationEventHandler : IMessageHandler<AppointmentCreatedIntegrationEvent>
{
    private readonly IRepository<Patient, PatientId> _patientRepository;
    private readonly ILogger<AppointmentCreatedIntegrationEventHandler> _logger;

    public AppointmentCreatedIntegrationEventHandler(
        IRepository<Patient, PatientId> patientRepository,
        ILogger<AppointmentCreatedIntegrationEventHandler> logger)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Required by IMessageHandler<T> - overload without metadata
    public async Task HandleAsync(AppointmentCreatedIntegrationEvent message, CancellationToken cancellationToken = default)
    {
        await HandleAsync(message, new MessageEnvelop(), cancellationToken);
    }

    // Required by IMessageHandler<T> - overload with metadata
    public async Task HandleAsync(AppointmentCreatedIntegrationEvent message, MessageEnvelop metadata, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Received AppointmentCreatedIntegrationEvent: AppointmentId={AppointmentId}, PatientId={PatientId}, CorrelationId={CorrelationId}",
            message.AppointmentId, message.PatientId, metadata.CorrelationId);

        try
        {
            // Get the patient
            var patientId = new PatientId(message.PatientId);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                _logger.LogWarning(
                    "Patient {PatientId} not found for appointment {AppointmentId}",
                    message.PatientId, message.AppointmentId);
                return;
            }

            // Log appointment details for the patient
            _logger.LogInformation(
                "Patient {PatientName} ({PatientId}) has appointment {AppointmentId} scheduled for {AppointmentDate} with Dr. {DoctorName} - Type: {AppointmentType}, Status: {Status}",
                patient.Name.FirstName + " " + patient.Name.LastName,
                message.PatientId,
                message.AppointmentId,
                message.AppointmentDate,
                message.DoctorName,
                message.AppointmentType,
                message.Status);

            // TODO: Add domain logic to update patient's appointment history
            // For example, you might add a method to the Patient aggregate:
            // patient.AddAppointmentHistory(message.AppointmentId, message.AppointmentDate, message.DoctorName, message.AppointmentType);

            // If patient aggregate was modified, save changes
            // await _patientRepository.UpdateAsync(patient, cancellationToken);

            _logger.LogInformation(
                "Successfully processed AppointmentCreatedIntegrationEvent for Patient {PatientId}",
                message.PatientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing AppointmentCreatedIntegrationEvent: AppointmentId={AppointmentId}, PatientId={PatientId}",
                message.AppointmentId, message.PatientId);
            throw; // Rethrow to trigger Kafka retry mechanism
        }
    }
}
