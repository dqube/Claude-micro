using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using BuildingBlocks.API.Responses.Base;
using System.Reflection;

namespace BuildingBlocks.API.OpenApi.Filters;

public class DefaultResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common response types if not already present
        AddDefaultResponses(operation, context);
        
        // Update response schemas to use consistent API response format
        UpdateResponseSchemas(operation, context);
        
        // Add examples for common responses
        AddResponseExamples(operation, context);
    }

    private static void AddDefaultResponses(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();
        
        // Add 400 Bad Request for methods that accept input
        if (ShouldAddBadRequestResponse(httpMethod) && !operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request - Invalid input data",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository)
                    }
                }
            });
        }

        // Add 404 Not Found for methods that work with specific resources
        if (ShouldAddNotFoundResponse(httpMethod, context) && !operation.Responses.ContainsKey("404"))
        {
            operation.Responses.Add("404", new OpenApiResponse
            {
                Description = "Not Found - Resource not found",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository)
                    }
                }
            });
        }

        // Add 422 Validation Error for methods that accept input
        if (ShouldAddValidationErrorResponse(httpMethod) && !operation.Responses.ContainsKey("422"))
        {
            operation.Responses.Add("422", new OpenApiResponse
            {
                Description = "Validation Error - Input validation failed",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ValidationErrorResponse), context.SchemaRepository)
                    }
                }
            });
        }

        // Add 500 Internal Server Error (always)
        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error - An unexpected error occurred",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository)
                    }
                }
            });
        }

        // Add 429 Rate Limited
        if (!operation.Responses.ContainsKey("429"))
        {
            operation.Responses.Add("429", new OpenApiResponse
            {
                Description = "Too Many Requests - Rate limit exceeded",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository)
                    }
                }
            });
        }
    }

    private static void UpdateResponseSchemas(OpenApiOperation operation, OperationFilterContext context)
    {
        // Update success responses to use ApiResponse wrapper
        foreach (var response in operation.Responses)
        {
            var statusCode = int.Parse(response.Key);
            if (statusCode >= 200 && statusCode < 300 && response.Value.Content?.ContainsKey("application/json") == true)
            {
                var jsonContent = response.Value.Content["application/json"];
                var originalSchema = jsonContent.Schema;
                
                // Don't wrap if it's already an ApiResponse
                if (IsApiResponseType(originalSchema, context))
                    continue;
                
                // Wrap in ApiResponse<T>
                var apiResponseType = typeof(ApiResponse<>).MakeGenericType(GetSchemaType(originalSchema) ?? typeof(object));
                jsonContent.Schema = context.SchemaGenerator.GenerateSchema(apiResponseType, context.SchemaRepository);
            }
        }
    }

    private static void AddResponseExamples(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add examples for error responses
        if (operation.Responses.TryGetValue("400", out var badRequestResponse))
        {
            AddErrorExample(badRequestResponse, "Invalid request data", 400);
        }

        if (operation.Responses.TryGetValue("404", out var notFoundResponse))
        {
            AddErrorExample(notFoundResponse, "Resource not found", 404);
        }

        if (operation.Responses.TryGetValue("422", out var validationErrorResponse))
        {
            AddValidationErrorExample(validationErrorResponse);
        }

        if (operation.Responses.TryGetValue("500", out var serverErrorResponse))
        {
            AddErrorExample(serverErrorResponse, "An unexpected error occurred", 500);
        }
    }

    private static void AddErrorExample(OpenApiResponse response, string message, int statusCode)
    {
        if (response.Content?.TryGetValue("application/json", out var mediaType) == true)
        {
            mediaType.Example = Microsoft.OpenApi.Any.OpenApiAnyFactory.CreateFromJson($@"{{
                ""success"": false,
                ""message"": ""{message}"",
                ""correlationId"": ""12345678-1234-5678-9012-123456789012"",
                ""timestamp"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}""
            }}");
        }
    }

    private static void AddValidationErrorExample(OpenApiResponse response)
    {
        if (response.Content?.TryGetValue("application/json", out var mediaType) == true)
        {
            mediaType.Example = Microsoft.OpenApi.Any.OpenApiAnyFactory.CreateFromJson($@"{{
                ""success"": false,
                ""message"": ""Validation failed"",
                ""correlationId"": ""12345678-1234-5678-9012-123456789012"",
                ""timestamp"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}"",
                ""validationErrors"": {{
                    ""field1"": [""Field1 is required""],
                    ""field2"": [""Field2 must be a valid email""]
                }}
            }}");
        }
    }

    private static bool ShouldAddBadRequestResponse(string? httpMethod)
    {
        return httpMethod is "POST" or "PUT" or "PATCH";
    }

    private static bool ShouldAddNotFoundResponse(string? httpMethod, OperationFilterContext context)
    {
        // Add 404 for operations that work with specific resources (have ID parameters)
        var hasIdParameter = context.ApiDescription.ParameterDescriptions
            .Any(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
        
        return httpMethod is "GET" or "PUT" or "DELETE" or "PATCH" && hasIdParameter;
    }

    private static bool ShouldAddValidationErrorResponse(string? httpMethod)
    {
        return httpMethod is "POST" or "PUT" or "PATCH";
    }

    private static bool IsApiResponseType(OpenApiSchema? schema, OperationFilterContext context)
    {
        if (schema?.Reference?.Id == null)
            return false;
            
        return schema.Reference.Id.StartsWith("ApiResponse") ||
               schema.Reference.Id.StartsWith("PagedResponse") ||
               schema.Reference.Id.StartsWith("ErrorResponse");
    }

    private static Type? GetSchemaType(OpenApiSchema? schema)
    {
        // This is a simplified implementation
        // In a real scenario, you'd need to map OpenAPI schemas back to .NET types
        return typeof(object);
    }
}