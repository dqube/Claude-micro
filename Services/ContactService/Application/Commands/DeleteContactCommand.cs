using BuildingBlocks.Application.CQRS.Commands;

namespace ContactService.Application.Commands;

public class DeleteContactCommand : CommandBase
{
    public Guid ContactId { get; init; }

    public DeleteContactCommand(Guid contactId)
    {
        ContactId = contactId;
    }
}