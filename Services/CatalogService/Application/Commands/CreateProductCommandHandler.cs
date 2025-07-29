using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Domain.Exceptions;

namespace CatalogService.Application.Commands;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sku = SKU.From(request.SKU);
        var categoryId = CategoryId.From(request.CategoryId);
        var basePrice = Price.From(request.BasePrice);
        var costPrice = Price.From(request.CostPrice);

        if (await _productRepository.SKUExistsAsync(sku, cancellationToken))
        {
            throw new DuplicateSKUException(sku);
        }

        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            throw new ProductCategoryNotFoundException(categoryId);
        }

        var product = new Product(
            ProductId.New(),
            sku,
            request.Name,
            categoryId,
            basePrice,
            costPrice,
            request.Description,
            request.IsTaxable);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(product, category);
    }

    private static ProductDto MapToDto(Product product, ProductCategory category)
    {
        return new ProductDto
        {
            Id = product.Id.Value,
            SKU = product.SKU.Value,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId.Value,
            CategoryName = category.Name,
            BasePrice = product.BasePrice.Value,
            CostPrice = product.CostPrice.Value,
            IsTaxable = product.IsTaxable,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Barcodes = product.Barcodes.Select(b => new ProductBarcodeDto
            {
                Id = b.Id.Value,
                BarcodeValue = b.BarcodeValue.Value,
                BarcodeType = b.BarcodeType.Name,
                CreatedAt = b.CreatedAt
            }).ToList(),
            CountryPricing = product.CountryPricing.Select(cp => new CountryPricingDto
            {
                Id = cp.Id.Value,
                CountryCode = cp.CountryCode.Value,
                Price = cp.Price.Value,
                EffectiveDate = cp.EffectiveDate,
                CreatedAt = cp.CreatedAt
            }).ToList()
        };
    }
}