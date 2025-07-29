using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;
using CatalogService.Domain.Repositories;
using CatalogService.Domain.Exceptions;

namespace CatalogService.Application.Commands;

public class CreateProductCategoryCommandHandler : ICommandHandler<CreateProductCategoryCommand, ProductCategoryDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCategoryCommandHandler(
        IProductCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDto> HandleAsync(CreateProductCategoryCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _categoryRepository.NameExistsAsync(request.Name, cancellationToken))
        {
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists");
        }

        CategoryId? parentCategoryId = null;
        ProductCategory? parentCategory = null;

        if (request.ParentCategoryId.HasValue)
        {
            parentCategoryId = CategoryId.From(request.ParentCategoryId.Value);
            parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId, cancellationToken);
            if (parentCategory == null)
            {
                throw new ProductCategoryNotFoundException(parentCategoryId);
            }
        }

        var nextId = await GetNextCategoryIdAsync(cancellationToken);
        var category = new ProductCategory(
            CategoryId.From(nextId),
            request.Name,
            parentCategoryId);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(category, parentCategory);
    }

    private async Task<int> GetNextCategoryIdAsync(CancellationToken cancellationToken)
    {
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        return allCategories.Any() ? allCategories.Max(c => c.Id.Value) + 1 : 1;
    }

    private static ProductCategoryDto MapToDto(ProductCategory category, ProductCategory? parentCategory)
    {
        return new ProductCategoryDto
        {
            Id = category.Id.Value,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId?.Value,
            ParentCategoryName = parentCategory?.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            SubCategories = new List<ProductCategoryDto>(),
            ProductCount = 0
        };
    }
}