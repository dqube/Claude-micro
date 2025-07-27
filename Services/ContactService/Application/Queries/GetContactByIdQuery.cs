using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs;

namespace ContactService.Application.Queries;

public class GetContactByIdQuery : QueryBase<ContactDto?>
{
    public Guid ContactId { get; init; }

    public GetContactByIdQuery(Guid contactId)
    {
        ContactId = contactId;
    }
}