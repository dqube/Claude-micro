using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Extensions;

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        // GlobalExceptionMiddleware should not be registered as a service
        // It is instantiated directly by the middleware pipeline
        
        return services;
    }

    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }

    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature?.Error != null)
                {
                    var logger = context.RequestServices.GetService<ILogger<GlobalExceptionMiddleware>>();
                    var correlationId = CorrelationHelper.GetCorrelationId(context);
                    
                    logger?.LogError(exceptionHandlerFeature.Error, 
                        "An unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorResponse = ResponseHelper.Error(
                        "An internal server error occurred.",
                        "INTERNAL_SERVER_ERROR",
                        correlationId: correlationId);

                    await context.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(errorResponse, new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        }));
                }
            });
        });

        return app;
    }

    public static WebApplication UseCustomErrorHandler(this WebApplication app, Func<HttpContext, Exception, Task> errorHandler)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature?.Error != null)
                {
                    await errorHandler(context, exceptionHandlerFeature.Error);
                }
            });
        });

        return app;
    }

    public static WebApplication UseDevelopmentErrorHandling(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseApiErrorHandling();
        }

        return app;
    }

    public static WebApplication UseProductionErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var logger = context.RequestServices.GetService<ILogger<GlobalExceptionMiddleware>>();
                var correlationId = CorrelationHelper.GetCorrelationId(context);
                
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = ResponseHelper.Error(
                    "An unexpected error occurred. Please try again later.",
                    "INTERNAL_SERVER_ERROR",
                    correlationId: correlationId);

                logger?.LogError("Production error occurred. CorrelationId: {CorrelationId}", correlationId);

                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(errorResponse));
            });
        });

        return app;
    }

    public static IResult HandleValidationError(IDictionary<string, string[]> errors, string? correlationId = null)
    {
        return ResponseFactory.ValidationError(errors, correlationId);
    }

    public static IResult HandleNotFound(string resource, string? correlationId = null)
    {
        return ResponseFactory.NotFound(resource, correlationId);
    }

    public static IResult HandleBadRequest(string message, string? correlationId = null)
    {
        return ResponseFactory.BadRequest(message, correlationId);
    }

    public static IResult HandleUnauthorized(string? message = null, string? correlationId = null)
    {
        return ResponseFactory.Unauthorized(message, correlationId);
    }

    public static IResult HandleForbidden(string? message = null, string? correlationId = null)
    {
        return ResponseFactory.Forbidden(message, correlationId);
    }

    public static IResult HandleConflict(string message, string? correlationId = null)
    {
        return ResponseFactory.Conflict(message, correlationId);
    }

    public static IResult HandleInternalError(string? message = null, string? correlationId = null)
    {
        return ResponseFactory.InternalServerError(message, correlationId);
    }
}