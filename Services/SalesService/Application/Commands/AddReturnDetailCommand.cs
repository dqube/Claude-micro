using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class AddReturnDetailCommand : CommandBase<ReturnDetailDto>
{
    public Guid ReturnId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public bool Restock { get; init; } = true;

    public AddReturnDetailCommand(Guid returnId, Guid productId, int quantity, string reason, bool restock = true)
    {
        ReturnId = returnId;
        ProductId = productId;
        Quantity = quantity;
        Reason = reason;
        Restock = restock;
    }
}