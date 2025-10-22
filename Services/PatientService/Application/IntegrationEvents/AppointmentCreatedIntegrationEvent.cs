using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Messages;

namespace PatientService.Application.IntegrationEvents;

/// <summary>
/// Integration event received when an appointment is created in AppointmentService
/// PatientService subscribes to this event to update patient appointment history
/// </summary>
public class AppointmentCreatedIntegrationEvent : IntegrationEventBase, IMessage
{
    public Guid AppointmentId { get; init; }
    public Guid PatientId { get; init; }
    public string DoctorName { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string AppointmentType { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }

    // IMessage implementation
    Guid IMessage.Id => Id;
    DateTime IMessage.Timestamp => OccurredOn;

    public AppointmentCreatedIntegrationEvent(
        Guid appointmentId,
        Guid patientId,
        string doctorName,
        DateTime appointmentDate,
        string appointmentType,
        string status,
        DateTime createdAt)
        : base("AppointmentService", Guid.NewGuid().ToString())
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorName = doctorName;
        AppointmentDate = appointmentDate;
        AppointmentType = appointmentType;
        Status = status;
        CreatedAt = createdAt;
    }
}
