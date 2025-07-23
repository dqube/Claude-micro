using AuthService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Queries;

namespace AuthService.Application.Queries;

public class GetUserByIdQuery : QueryBase<UserDto>
{
    public Guid UserId { get; init; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}
