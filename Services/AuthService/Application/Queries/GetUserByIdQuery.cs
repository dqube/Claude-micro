using BuildingBlocks.Application.CQRS.Queries;
using AuthService.Application.DTOs;

namespace AuthService.Application.Queries;

public class GetUserByIdQuery : QueryBase<UserDto>
{
    public Guid UserId { get; init; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}