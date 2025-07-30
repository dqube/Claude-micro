using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetSupplierContactsQuery : QueryBase<IEnumerable<SupplierContactDto>>
{
    public Guid SupplierId { get; init; }

    public GetSupplierContactsQuery(Guid supplierId)
    {
        SupplierId = supplierId;
    }
} 