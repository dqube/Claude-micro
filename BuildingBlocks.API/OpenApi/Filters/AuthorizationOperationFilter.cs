using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace BuildingBlocks.API.OpenApi.Filters;

public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo
            .GetCustomAttributes<AuthorizeAttribute>(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>(true) ?? Array.Empty<AuthorizeAttribute>())
            .ToList();

        var allowAnonymousAttributes = context.MethodInfo
            .GetCustomAttributes<AllowAnonymousAttribute>(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes<AllowAnonymousAttribute>(true) ?? Array.Empty<AllowAnonymousAttribute>())
            .ToList();

        // If AllowAnonymous is present, no security is required
        if (allowAnonymousAttributes.Any())
        {
            return;
        }

        // If no authorization attributes, check if endpoint requires authorization
        var requiresAuth = authAttributes.Any() || HasAuthorizeMetadata(context);
        
        if (!requiresAuth)
        {
            return;
        }

        // Add security requirement
        var securityRequirements = new List<OpenApiSecurityRequirement>();

        // JWT Bearer authentication
        var jwtScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
        securityRequirements.Add(new OpenApiSecurityRequirement
        {
            [jwtScheme] = Array.Empty<string>()
        });

        // API Key authentication
        var apiKeyScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            }
        };
        securityRequirements.Add(new OpenApiSecurityRequirement
        {
            [apiKeyScheme] = Array.Empty<string>()
        });

        operation.Security = securityRequirements;

        // Add responses for authorization
        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized - Authentication is required"
            });
        }

        if (!operation.Responses.ContainsKey("403"))
        {
            operation.Responses.Add("403", new OpenApiResponse
            {
                Description = "Forbidden - Insufficient permissions"
            });
        }

        // Add role/policy information to summary
        var roleRequirements = authAttributes
            .Where(a => !string.IsNullOrEmpty(a.Roles))
            .SelectMany(a => a.Roles!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(r => r.Trim())
            .Distinct()
            .ToList();

        var policyRequirements = authAttributes
            .Where(a => !string.IsNullOrEmpty(a.Policy))
            .Select(a => a.Policy!.Trim())
            .Distinct()
            .ToList();

        if (roleRequirements.Any() || policyRequirements.Any())
        {
            var requirements = new List<string>();
            
            if (roleRequirements.Any())
            {
                requirements.Add($"Roles: {string.Join(", ", roleRequirements)}");
            }
            
            if (policyRequirements.Any())
            {
                requirements.Add($"Policies: {string.Join(", ", policyRequirements)}");
            }

            var requirementsText = string.Join("; ", requirements);
            
            if (!string.IsNullOrEmpty(operation.Summary))
            {
                operation.Summary += $" (Required: {requirementsText})";
            }
            else
            {
                operation.Summary = $"Required: {requirementsText}";
            }
        }
    }

    private static bool HasAuthorizeMetadata(OperationFilterContext context)
    {
        // Check for authorization metadata in minimal APIs
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        return endpointMetadata?.Any(m => m is AuthorizeAttribute) == true &&
               endpointMetadata?.Any(m => m is AllowAnonymousAttribute) != true;
    }
}