using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Utilities.Factories;
using System.Text.Json;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class ErrorHandlingExtensions
{
    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseGlobalExceptionHandler();
        }
        
        return app;
    }

    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }

    public static WebApplication UseExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    var errorResponse = ErrorFactory.CreateFromException(exception);
                    
                    context.Response.StatusCode = GetStatusCodeFromException(exception);
                    context.Response.ContentType = "application/json";
                    
                    var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    await context.Response.WriteAsync(json);
                }
            });
        });
        
        return app;
    }

    public static WebApplication UseProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    var problemDetails = ErrorFactory.CreateProblemDetailsFromException(exception, context);
                    
                    context.Response.StatusCode = problemDetails.Status ?? 500;
                    context.Response.ContentType = "application/problem+json";
                    
                    var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    await context.Response.WriteAsync(json);
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
            app.UseGlobalExceptionHandler();
        }
        
        return app;
    }

    public static WebApplication UseProductionErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
        
        return app;
    }

    public static WebApplication UseCustomExceptionHandler(this WebApplication app, Func<Microsoft.AspNetCore.Http.HttpContext, Exception, Task> handler)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    await handler(context, exception);
                }
            });
        });
        
        return app;
    }

    public static WebApplication UseStatusCodePages(this WebApplication app)
    {
        app.UseStatusCodePages(async context =>
        {
            var statusCode = context.HttpContext.Response.StatusCode;
            var message = GetMessageForStatusCode(statusCode);
            
            var errorResponse = new
            {
                success = false,
                message = message,
                statusCode = statusCode,
                timestamp = DateTime.UtcNow
            };
            
            context.HttpContext.Response.ContentType = "application/json";
            
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.HttpContext.Response.WriteAsync(json);
        });
        
        return app;
    }

    private static int GetStatusCodeFromException(Exception exception)
    {
        return exception switch
        {
            ArgumentException => 400,
            ArgumentNullException => 400,
            UnauthorizedAccessException => 401,
            NotImplementedException => 501,
            TimeoutException => 408,
            InvalidOperationException => 409,
            _ => 500
        };
    }

    private static string GetMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            408 => "Request Timeout",
            409 => "Conflict",
            429 => "Too Many Requests",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            _ => "An error occurred"
        };
    }
}