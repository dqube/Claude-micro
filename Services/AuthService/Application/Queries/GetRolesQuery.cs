using BuildingBlocks.Application.CQRS.Queries;
using AuthService.Application.DTOs;

namespace AuthService.Application.Queries;

public class GetRolesQuery : QueryBase<List<RoleDto>>
{
    public string? SearchTerm { get; init; }

    public GetRolesQuery(string? searchTerm = null)
    {
        SearchTerm = searchTerm;
    }
}