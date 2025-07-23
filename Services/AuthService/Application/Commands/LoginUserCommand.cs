using AuthService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands;

public class LoginUserCommand : CommandBase<LoginResponseDto>
{
    public LoginUserDto LoginUserDto { get; set; }

    public LoginUserCommand(LoginUserDto loginUserDto)
    {
        LoginUserDto = loginUserDto;
    }
}