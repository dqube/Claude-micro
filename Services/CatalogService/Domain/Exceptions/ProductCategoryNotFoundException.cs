using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Exceptions;

public class ProductCategoryNotFoundException : Exception
{
    public ProductCategoryNotFoundException(CategoryId categoryId) 
        : base($"Product category with ID '{categoryId.Value}' was not found")
    {
    }

    public ProductCategoryNotFoundException(string categoryName) 
        : base($"Product category with name '{categoryName}' was not found")
    {
    }
}