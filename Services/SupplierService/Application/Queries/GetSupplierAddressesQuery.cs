using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetSupplierAddressesQuery : QueryBase<IEnumerable<SupplierAddressDto>>
{
    public Guid SupplierId { get; init; }

    public GetSupplierAddressesQuery(Guid supplierId)
    {
        SupplierId = supplierId;
    }
} 