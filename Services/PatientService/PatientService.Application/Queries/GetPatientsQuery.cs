using BuildingBlocks.Application.CQRS.Queries;
using PatientService.Application.DTOs;

namespace PatientService.Application.Queries;

public class GetPatientsQuery : PagedQuery<PatientDto>
{
    public string? SearchTerm { get; }
    public bool? IsActive { get; }
    public string? Gender { get; }
    public int? MinAge { get; }
    public int? MaxAge { get; }

    public GetPatientsQuery(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        string? gender = null,
        int? minAge = null,
        int? maxAge = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
        Gender = gender;
        MinAge = minAge;
        MaxAge = maxAge;
    }
}