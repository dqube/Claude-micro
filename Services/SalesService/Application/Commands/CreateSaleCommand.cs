using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class CreateSaleCommand : CommandBase<SaleDto>
{
    public int StoreId { get; init; }
    public Guid EmployeeId { get; init; }
    public int RegisterId { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public Guid? CustomerId { get; init; }

    public CreateSaleCommand(int storeId, Guid employeeId, int registerId, string receiptNumber, Guid? customerId = null)
    {
        StoreId = storeId;
        EmployeeId = employeeId;
        RegisterId = registerId;
        ReceiptNumber = receiptNumber;
        CustomerId = customerId;
    }
}