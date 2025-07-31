using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetReturnByIdQuery : QueryBase<ReturnDto?>
{
    public Guid ReturnId { get; init; }

    public GetReturnByIdQuery(Guid returnId)
    {
        ReturnId = returnId;
    }
}