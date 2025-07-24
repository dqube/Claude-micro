using BuildingBlocks.Application.CQRS.Queries;
using AuthService.Application.DTOs;

namespace AuthService.Application.Queries;

public class GetUserByEmailQuery : QueryBase<UserDto>
{
    public string Email { get; init; } = string.Empty;

    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }
}