using BuildingBlocks.Application.CQRS.Queries;
using PatientService.Application.DTOs;

namespace PatientService.Application.Queries;

public class GetPatientByIdQuery : QueryBase<PatientDto>
{
    public Guid PatientId { get; }

    public GetPatientByIdQuery(Guid patientId)
    {
        PatientId = patientId;
    }
}