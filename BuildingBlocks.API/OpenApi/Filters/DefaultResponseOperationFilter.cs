using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.API.OpenApi.Filters;

public class DefaultResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= new OpenApiResponses();

        // Add default error responses if not already present
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request - Invalid request parameters"
            });
        }

        if (!operation.Responses.ContainsKey("404"))
        {
            operation.Responses.Add("404", new OpenApiResponse
            {
                Description = "Not Found - Resource not found"
            });
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error - An unexpected error occurred"
            });
        }

        // Add validation error response for POST/PUT operations
        if (IsWriteOperation(context) && !operation.Responses.ContainsKey("422"))
        {
            operation.Responses.Add("422", new OpenApiResponse
            {
                Description = "Unprocessable Entity - Validation failed"
            });
        }
    }

    private static bool IsWriteOperation(OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant();
        return httpMethod == "POST" || httpMethod == "PUT" || httpMethod == "PATCH";
    }
}