using BuildingBlocks.Domain.Exceptions;

namespace AuthService.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid userId) 
        : base($"User with ID '{userId}' not found.")
    {
    }
}
