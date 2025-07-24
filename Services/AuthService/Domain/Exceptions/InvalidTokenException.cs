using BuildingBlocks.Domain.Exceptions;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Exceptions;

public class InvalidTokenException : DomainException
{
    public InvalidTokenException() : base("Token is invalid or expired")
    {
    }

    public InvalidTokenException(string message) 
        : base(message)
    {
    }

    public InvalidTokenException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public InvalidTokenException(TokenId tokenId) 
        : base($"Token with ID '{tokenId?.Value}' is invalid or expired")
    {
    }
}