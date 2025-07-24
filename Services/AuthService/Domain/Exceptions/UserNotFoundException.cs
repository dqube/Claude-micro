using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Exceptions;

public class UserNotFoundException : AggregateNotFoundException
{
    public UserNotFoundException() : base("User was not found")
    {
    }

    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UserNotFoundException(UserId userId) 
        : base($"User with ID '{userId?.Value}' was not found")
    {
    }

    public UserNotFoundException(string username, bool isUsername) 
        : base($"User with username '{username}' was not found")
    {
    }

    public UserNotFoundException(Email email) 
        : base($"User with email '{email?.Value}' was not found")
    {
    }
}