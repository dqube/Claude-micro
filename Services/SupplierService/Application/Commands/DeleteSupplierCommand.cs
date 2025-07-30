using BuildingBlocks.Application.CQRS.Commands;

namespace SupplierService.Application.Commands;

public class DeleteSupplierCommand : CommandBase<bool>
{
    public Guid Id { get; init; }

    public DeleteSupplierCommand(Guid id)
    {
        Id = id;
    }
} 