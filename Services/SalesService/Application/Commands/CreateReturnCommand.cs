using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class CreateReturnCommand : CommandBase<ReturnDto>
{
    public Guid SaleId { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid? CustomerId { get; init; }

    public CreateReturnCommand(Guid saleId, Guid employeeId, Guid? customerId = null)
    {
        SaleId = saleId;
        EmployeeId = employeeId;
        CustomerId = customerId;
    }
}