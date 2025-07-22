using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using BuildingBlocks.API.OpenApi.Filters;
using BuildingBlocks.API.OpenApi.Configuration;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.OpenApi.Extensions;

public static class OpenApiExtensions
{
    public static SwaggerGenOptions AddStandardOpenApiConfiguration(this SwaggerGenOptions options, ApiOptions apiOptions)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = apiOptions.Title,
            Version = apiOptions.Version,
            Description = apiOptions.Description,
            TermsOfService = !string.IsNullOrEmpty(apiOptions.TermsOfService) ? new Uri(apiOptions.TermsOfService) : null,
            Contact = apiOptions.Contact != null ? new OpenApiContact
            {
                Name = apiOptions.Contact.Name,
                Email = apiOptions.Contact.Email,
                Url = !string.IsNullOrEmpty(apiOptions.Contact.Url) ? new Uri(apiOptions.Contact.Url) : null
            } : null,
            License = apiOptions.License != null ? new OpenApiLicense
            {
                Name = apiOptions.License.Name,
                Url = !string.IsNullOrEmpty(apiOptions.License.Url) ? new Uri(apiOptions.License.Url) : null
            } : null
        });

        // Add standard security definitions
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token in the format: Bearer {your token}",
            In = ParameterLocation.Header,
            Name = "Authorization"
        });

        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-API-Key",
            Description = "API Key needed to access the endpoints"
        });

        // Add operation filters
        options.OperationFilter<AuthorizationOperationFilter>();
        options.OperationFilter<DefaultResponseOperationFilter>();

        return options;
    }

    public static SwaggerGenOptions AddApiDocumentationOptions(this SwaggerGenOptions options, ApiDocumentationOptions docOptions)
    {
        // Configure security schemes
        if (docOptions.Security.EnableJwtBearer)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = docOptions.Security.JwtBearerDescription
            });
        }

        if (docOptions.Security.EnableApiKey)
        {
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = docOptions.Security.ApiKeyHeaderName,
                Description = docOptions.Security.ApiKeyDescription
            });
        }

        if (docOptions.Security.EnableOAuth2)
        {
            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(docOptions.Security.OAuth2.AuthorizationUrl),
                        TokenUrl = new Uri(docOptions.Security.OAuth2.TokenUrl),
                        RefreshUrl = !string.IsNullOrEmpty(docOptions.Security.OAuth2.RefreshUrl) 
                            ? new Uri(docOptions.Security.OAuth2.RefreshUrl) : null,
                        Scopes = docOptions.Security.OAuth2.Scopes
                    }
                }
            });
        }

        // Add servers
        foreach (var server in docOptions.Servers)
        {
            var openApiServer = new OpenApiServer
            {
                Url = server.Url,
                Description = server.Description
            };

            foreach (var variable in server.Variables)
            {
                openApiServer.Variables.Add(variable.Key, new OpenApiServerVariable
                {
                    Enum = variable.Value.Enum.ToList(),
                    Default = variable.Value.Default,
                    Description = variable.Value.Description
                });
            }
        }

        // Include XML comments
        if (docOptions.IncludeXmlComments)
        {
            foreach (var xmlFile in docOptions.XmlCommentsFiles)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }

            // Include XML comments for the current assembly
            var currentAssemblyXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var currentAssemblyXmlPath = Path.Combine(AppContext.BaseDirectory, currentAssemblyXmlFile);
            if (File.Exists(currentAssemblyXmlPath))
            {
                options.IncludeXmlComments(currentAssemblyXmlPath);
            }
        }

        // Configure tags
        if (docOptions.SortTagsAlphabetically)
        {
            options.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
            options.OrderActionsBy(api => $"{api.GroupName}_{api.RelativePath}");
        }

        return options;
    }

    public static SwaggerGenOptions AddCustomSchemaIds(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(type =>
        {
            var name = type.Name;
            
            // Handle generic types
            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments()
                    .Select(arg => arg.Name)
                    .ToArray();
                    
                name = $"{name.Split('`')[0]}Of{string.Join("And", genericArguments)}";
            }

            return name;
        });

        return options;
    }

    public static SwaggerGenOptions ConfigureForMinimalApis(this SwaggerGenOptions options)
    {
        // Configure Swagger for Minimal APIs
        options.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
        options.OrderActionsBy(api => $"{api.GroupName}_{api.RelativePath}");
        
        // Support for nullable reference types
        options.SupportNonNullableReferenceTypes();
        
        // Use all of controller for schema id
        options.CustomSchemaIds(x => x.FullName);
        
        return options;
    }

    public static SwaggerGenOptions AddVersionedDocuments(this SwaggerGenOptions options, ApiOptions apiOptions, params string[] versions)
    {
        foreach (var version in versions)
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = apiOptions.Title,
                Version = version,
                Description = $"{apiOptions.Description} - Version {version}"
            });
        }

        // Configure version inclusion
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                return false;

            var versions = methodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .ToList() ?? new List<Microsoft.AspNetCore.Mvc.ApiVersion>();

            return versions.Any(v => $"v{v}" == docName);
        });

        return options;
    }

    public static SwaggerGenOptions AddResponseExamples(this SwaggerGenOptions options)
    {
        // Add schema filter for response examples
        options.SchemaFilter<ExampleSchemaFilter>();
        return options;
    }

    public static SwaggerGenOptions MapType<T>(this SwaggerGenOptions options, Func<OpenApiSchema> schemaFactory)
    {
        options.MapType<T>(schemaFactory);
        return options;
    }
}

public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Add examples for common types
        if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
        {
            schema.Example = Microsoft.OpenApi.Any.OpenApiAnyFactory.CreateFromJson($"\"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}\"");
        }
        else if (context.Type == typeof(Guid) || context.Type == typeof(Guid?))
        {
            schema.Example = Microsoft.OpenApi.Any.OpenApiAnyFactory.CreateFromJson($"\"{Guid.NewGuid()}\"");
        }
    }
}