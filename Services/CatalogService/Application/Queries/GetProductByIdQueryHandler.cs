using BuildingBlocks.Application.CQRS.Queries;
using CatalogService.Application.DTOs;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Queries;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var productId = ProductId.From(request.ProductId);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        
        if (product == null)
            return null;

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);

        return new ProductDto
        {
            Id = product.Id.Value,
            SKU = product.SKU.Value,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId.Value,
            CategoryName = category?.Name,
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