using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class ProcessReturnCommand : CommandBase<ReturnDto>
{
    public Guid ReturnId { get; init; }
    public decimal TotalRefund { get; init; }
    public Guid? ProcessedBy { get; init; }

    public ProcessReturnCommand(Guid returnId, decimal totalRefund, Guid? processedBy = null)
    {
        ReturnId = returnId;
        TotalRefund = totalRefund;
        ProcessedBy = processedBy;
    }
}