using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class ApplyDiscountCommandHandler : ICommandHandler<ApplyDiscountCommand, AppliedDiscountDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyDiscountCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppliedDiscountDto> HandleAsync(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var sale = await _saleRepository.GetWithDetailsAsync(saleId, cancellationToken);
        if (sale == null)
        {
            throw new SaleNotFoundException(saleId);
        }

        sale.ApplyDiscount(request.SaleDetailId, request.CampaignId, request.RuleId, request.DiscountAmount);
        
        _saleRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var appliedDiscount = sale.AppliedDiscounts.Last();
        return new AppliedDiscountDto
        {
            Id = appliedDiscount.Id.Value,
            SaleDetailId = appliedDiscount.SaleDetailId?.Value,
            SaleId = appliedDiscount.SaleId?.Value,
            CampaignId = appliedDiscount.CampaignId,
            RuleId = appliedDiscount.RuleId,
            DiscountAmount = appliedDiscount.DiscountAmount,
            CreatedAt = appliedDiscount.CreatedAt,
            CreatedBy = appliedDiscount.CreatedBy
        };
    }
}