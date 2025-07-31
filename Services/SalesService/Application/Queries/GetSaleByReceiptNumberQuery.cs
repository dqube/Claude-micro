using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSaleByReceiptNumberQuery : QueryBase<SaleDto?>
{
    public string ReceiptNumber { get; init; }

    public GetSaleByReceiptNumberQuery(string receiptNumber)
    {
        ReceiptNumber = receiptNumber;
    }
}