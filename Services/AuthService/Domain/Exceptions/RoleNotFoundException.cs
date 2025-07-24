using BuildingBlocks.Domain.Exceptions;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Exceptions;

public class RoleNotFoundException : AggregateNotFoundException
{
    public RoleNotFoundException() : base("Role was not found")
    {
    }

    public RoleNotFoundException(string message) : base(message)
    {
    }

    public RoleNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RoleNotFoundException(RoleId roleId) 
        : base($"Role with ID '{roleId?.Value}' was not found")
    {
    }

    public RoleNotFoundException(string roleName, bool isRoleName) 
        : base($"Role with name '{roleName}' was not found")
    {
    }
}