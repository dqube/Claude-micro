using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class CompleteSaleCommand : CommandBase<SaleDto>
{
    public Guid SaleId { get; init; }
    public Guid? CompletedBy { get; init; }

    public CompleteSaleCommand(Guid saleId, Guid? completedBy = null)
    {
        SaleId = saleId;
        CompletedBy = completedBy;
    }
}