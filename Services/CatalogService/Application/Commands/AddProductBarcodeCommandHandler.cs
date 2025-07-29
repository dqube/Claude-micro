using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Domain.Exceptions;

namespace CatalogService.Application.Commands;

public class AddProductBarcodeCommandHandler : ICommandHandler<AddProductBarcodeCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductBarcodeCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AddProductBarcodeCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var productId = ProductId.From(request.ProductId);
        var barcodeValue = BarcodeValue.From(request.BarcodeValue);
        var barcodeType = BarcodeType.FromName(request.BarcodeType);

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        if (await _productRepository.BarcodeExistsAsync(barcodeValue, cancellationToken))
        {
            throw new DuplicateBarcodeException(barcodeValue);
        }

        product.AddBarcode(barcodeValue, barcodeType, Guid.Empty);
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}