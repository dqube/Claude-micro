using BuildingBlocks.Application.CQRS.Commands;

namespace PatientService.Application.Commands;

public class UpdatePatientContactCommand : CommandBase
{
    public Guid PatientId { get; }
    public string Email { get; }
    public string? PhoneNumber { get; }

    public UpdatePatientContactCommand(Guid patientId, string email, string? phoneNumber = null)
    {
        PatientId = patientId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}