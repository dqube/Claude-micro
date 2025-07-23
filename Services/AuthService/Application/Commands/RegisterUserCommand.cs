using AuthService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands;

public class RegisterUserCommand : CommandBase<UserDto>
{
    public RegisterUserDto RegisterUserDto { get; set; }

    public RegisterUserCommand(RegisterUserDto registerUserDto)
    {
        RegisterUserDto = registerUserDto;
    }
}