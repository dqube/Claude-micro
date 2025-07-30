using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetSuppliersByNameQuery : QueryBase<IEnumerable<SupplierDto>>
{
    public string Name { get; init; } = string.Empty;

    public GetSuppliersByNameQuery(string name)
    {
        Name = name;
    }
} 