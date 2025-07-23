using BuildingBlocks.Application.CQRS.Commands;

namespace PatientService.Application.Commands;

public class UpdatePatientContactCommand : CommandBase
{
    public Guid PatientId { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }

    public UpdatePatientContactCommand(Guid patientId, string email, string? phoneNumber = null)
    {
        PatientId = patientId;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}