using BuildingBlocks.Application.CQRS.Queries;
using CatalogService.Application.DTOs;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Queries;

public class GetProductCategoriesQueryHandler : IQueryHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public GetProductCategoriesQueryHandler(
        IProductCategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<List<ProductCategoryDto>> HandleAsync(GetProductCategoriesQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IEnumerable<Domain.Entities.ProductCategory> categories;

        if (request.ParentCategoryId.HasValue)
        {
            var parentId = CategoryId.From(request.ParentCategoryId.Value);
            categories = await _categoryRepository.GetByParentCategoryIdAsync(parentId, cancellationToken);
        }
        else if (request.IncludeHierarchy)
        {
            categories = await _categoryRepository.GetRootCategoriesAsync(cancellationToken);
        }
        else
        {
            categories = await _categoryRepository.GetAllAsync(cancellationToken);
        }

        var categoryList = categories.OrderBy(c => c.Name).ToList();
        var result = new List<ProductCategoryDto>();

        foreach (var category in categoryList)
        {
            var dto = await MapToDtoAsync(category, cancellationToken);
            
            if (request.IncludeHierarchy && category.ParentCategoryId == null)
            {
                dto = dto with { SubCategories = await GetSubCategoriesAsync(category.Id, cancellationToken) };
            }

            result.Add(dto);
        }

        return result;
    }

    private async Task<ProductCategoryDto> MapToDtoAsync(Domain.Entities.ProductCategory category, CancellationToken cancellationToken)
    {
        var productCount = (await _productRepository.GetByCategoryIdAsync(category.Id, cancellationToken)).Count();
        
        Domain.Entities.ProductCategory? parentCategory = null;
        if (category.ParentCategoryId != null)
        {
            parentCategory = await _categoryRepository.GetByIdAsync(category.ParentCategoryId, cancellationToken);
        }

        return new ProductCategoryDto
        {
            Id = category.Id.Value,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId?.Value,
            ParentCategoryName = parentCategory?.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            SubCategories = new List<ProductCategoryDto>(),
            ProductCount = productCount
        };
    }

    private async Task<List<ProductCategoryDto>> GetSubCategoriesAsync(CategoryId parentId, CancellationToken cancellationToken)
    {
        var subCategories = await _categoryRepository.GetByParentCategoryIdAsync(parentId, cancellationToken);
        var result = new List<ProductCategoryDto>();

        foreach (var subCategory in subCategories.OrderBy(c => c.Name))
        {
            var dto = await MapToDtoAsync(subCategory, cancellationToken);
            dto = dto with { SubCategories = await GetSubCategoriesAsync(subCategory.Id, cancellationToken) };
            result.Add(dto);
        }

        return result;
    }
}