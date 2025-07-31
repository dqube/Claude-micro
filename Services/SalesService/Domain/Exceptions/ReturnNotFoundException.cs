using BuildingBlocks.Domain.Exceptions;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Exceptions;

public class ReturnNotFoundException : DomainException
{
    public ReturnNotFoundException(ReturnId returnId) 
        : base($"Return with id '{returnId.Value}' was not found")
    {
    }
}