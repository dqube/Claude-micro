using BuildingBlocks.Domain.Exceptions;

namespace SalesService.Domain.Exceptions;

public class InvalidSaleOperationException : DomainException
{
    public InvalidSaleOperationException(string message) : base(message)
    {
    }
}