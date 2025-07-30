using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class DeactivateSupplierCommand : CommandBase<SupplierDto>
{
    public Guid Id { get; init; }

    public DeactivateSupplierCommand(Guid id)
    {
        Id = id;
    }
} 