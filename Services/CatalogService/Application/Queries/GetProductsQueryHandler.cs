using BuildingBlocks.Application.CQRS.Queries;
using CatalogService.Application.DTOs;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Queries;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var allProducts = await _productRepository.GetAllAsync(cancellationToken);
        var productsQuery = allProducts.AsQueryable();

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            productsQuery = productsQuery.Where(p =>
                p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.SKU.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (p.Description != null && p.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (request.CategoryId.HasValue)
        {
            var categoryId = CategoryId.From(request.CategoryId.Value);
            productsQuery = productsQuery.Where(p => p.CategoryId.Equals(categoryId));
        }

        if (request.IsTaxable.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.IsTaxable == request.IsTaxable.Value);
        }

        if (request.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.BasePrice.Value >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.BasePrice.Value <= request.MaxPrice.Value);
        }

        // Apply sorting
        var sortedProducts = request.SortBy.ToLowerInvariant() switch
        {
            "name" => request.SortDescending
                ? productsQuery.OrderByDescending(p => p.Name)
                : productsQuery.OrderBy(p => p.Name),
            "sku" => request.SortDescending
                ? productsQuery.OrderByDescending(p => p.SKU.Value)
                : productsQuery.OrderBy(p => p.SKU.Value),
            "baseprice" => request.SortDescending
                ? productsQuery.OrderByDescending(p => p.BasePrice.Value)
                : productsQuery.OrderBy(p => p.BasePrice.Value),
            "createdat" or _ => request.SortDescending
                ? productsQuery.OrderByDescending(p => p.CreatedAt)
                : productsQuery.OrderBy(p => p.CreatedAt)
        };

        // Apply pagination
        var totalCount = productsQuery.Count();
        var pagedItems = sortedProducts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get category information for mapping
        var categoryIds = pagedItems.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await Task.WhenAll(categoryIds.Select(async id => 
            await _categoryRepository.GetByIdAsync(id, cancellationToken)));
        var categoryDict = categories.Where(c => c != null).ToDictionary(c => c!.Id, c => c!.Name);

        var dtos = pagedItems.Select(product => new ProductDto
        {
            Id = product.Id.Value,
            SKU = product.SKU.Value,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId.Value,
            CategoryName = categoryDict.GetValueOrDefault(product.CategoryId),
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
        }).ToList();

        return new PagedResult<ProductDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}