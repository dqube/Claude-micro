using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IReadOnlyRepository<User, UserId> _userRepository;

    public GetUserByIdQueryHandler(IReadOnlyRepository<User, UserId> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        return new UserDto
        {
            UserId = user.Id.Value,
            Username = user.Username.Value,
            Email = user.Email.Value,
            IsActive = user.IsActive
        };
    }
}