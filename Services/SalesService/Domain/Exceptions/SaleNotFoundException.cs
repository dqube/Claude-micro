using BuildingBlocks.Domain.Exceptions;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Exceptions;

public class SaleNotFoundException : DomainException
{
    public SaleNotFoundException(SaleId saleId) 
        : base($"Sale with id '{saleId.Value}' was not found")
    {
    }
}