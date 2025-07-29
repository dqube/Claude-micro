using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Domain.Exceptions;

namespace CatalogService.Application.Commands;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var productId = ProductId.From(request.ProductId);
        var categoryId = CategoryId.From(request.CategoryId);
        var basePrice = Price.From(request.BasePrice);
        var costPrice = Price.From(request.CostPrice);

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            throw new ProductCategoryNotFoundException(categoryId);
        }

        product.UpdateBasicInfo(request.Name, request.Description, Guid.Empty);
        product.UpdatePricing(basePrice, costPrice, Guid.Empty);
        product.MoveToCategory(categoryId, Guid.Empty);
        product.UpdateTaxability(request.IsTaxable, Guid.Empty);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}