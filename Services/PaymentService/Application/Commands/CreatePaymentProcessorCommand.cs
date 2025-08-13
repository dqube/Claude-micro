using BuildingBlocks.Application.CQRS.Commands;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands;

public class CreatePaymentProcessorCommand : CommandBase<PaymentProcessorDto>
{
    public string Name { get; init; } = string.Empty;
    public decimal CommissionRate { get; init; }
    public bool IsActive { get; init; } = true;

    public CreatePaymentProcessorCommand(string name, decimal commissionRate = 0, bool isActive = true)
    {
        Name = name;
        CommissionRate = commissionRate;
        IsActive = isActive;
    }
} 