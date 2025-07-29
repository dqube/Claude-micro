using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Common;
using CustomerService.Application.DTOs;

namespace CustomerService.Application.Queries;

public class GetCustomersQuery : QueryBase<PagedResult<CustomerDto>>
{
    public int Page { get; }
    public int PageSize { get; }
    public string? SearchTerm { get; }
    public string? CountryCode { get; }
    public bool? IsMembershipActive { get; }

    public GetCustomersQuery(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? countryCode = null,
        bool? isMembershipActive = null)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
        SearchTerm = searchTerm;
        CountryCode = countryCode;
        IsMembershipActive = isMembershipActive;
    }
} 