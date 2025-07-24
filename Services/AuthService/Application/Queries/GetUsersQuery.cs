using BuildingBlocks.Application.CQRS.Queries;
using AuthService.Application.DTOs;

namespace AuthService.Application.Queries;

public class GetUsersQuery : PagedQuery<UserDto>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsLocked { get; init; }

    public GetUsersQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, bool? isLocked = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
        IsLocked = isLocked;
    }
}