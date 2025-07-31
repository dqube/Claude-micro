using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Entities;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class CreateSaleCommandHandler : ICommandHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSaleCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SaleDto> HandleAsync(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var receiptNumber = ReceiptNumber.From(request.ReceiptNumber);
        var existingSale = await _saleRepository.GetByReceiptNumberAsync(receiptNumber, cancellationToken);
        if (existingSale != null)
        {
            throw new InvalidOperationException($"Sale with receipt number '{request.ReceiptNumber}' already exists");
        }

        var sale = new Sale(
            SaleId.New(),
            request.StoreId,
            request.EmployeeId,
            request.RegisterId,
            receiptNumber,
            request.CustomerId);

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SaleDto
        {
            Id = sale.Id.Value,
            StoreId = sale.StoreId,
            EmployeeId = sale.EmployeeId,
            CustomerId = sale.CustomerId,
            RegisterId = sale.RegisterId,
            TransactionTime = sale.TransactionTime,
            SubTotal = sale.SubTotal,
            DiscountTotal = sale.DiscountTotal,
            TaxAmount = sale.TaxAmount,
            TotalAmount = sale.TotalAmount,
            ReceiptNumber = sale.ReceiptNumber.Value,
            CreatedAt = sale.CreatedAt,
            CreatedBy = sale.CreatedBy
        };
    }
}