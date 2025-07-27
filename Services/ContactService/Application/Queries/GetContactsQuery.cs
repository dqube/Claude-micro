using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs;

namespace ContactService.Application.Queries;

public class GetContactsQuery : PagedQuery<ContactDto>
{
    public string? SearchTerm { get; init; }
    public string? ContactType { get; init; }
    public bool? IsActive { get; init; }

    public GetContactsQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? contactType = null,
        bool? isActive = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        Page = pageNumber;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDescending ? SortDirection.Descending : SortDirection.Ascending;
        SearchTerm = searchTerm;
        ContactType = contactType;
        IsActive = isActive;
    }
}