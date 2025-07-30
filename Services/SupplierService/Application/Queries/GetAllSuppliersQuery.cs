using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetAllSuppliersQuery : QueryBase<IEnumerable<SupplierDto>>
{
    public bool ActiveOnly { get; init; }

    public GetAllSuppliersQuery(bool activeOnly = true)
    {
        ActiveOnly = activeOnly;
    }
} 