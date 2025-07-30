using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class ActivateSupplierCommand : CommandBase<SupplierDto>
{
    public Guid Id { get; init; }

    public ActivateSupplierCommand(Guid id)
    {
        Id = id;
    }
} 