using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetSupplierByIdQuery : QueryBase<SupplierDto?>
{
    public Guid Id { get; init; }

    public GetSupplierByIdQuery(Guid id)
    {
        Id = id;
    }
} 