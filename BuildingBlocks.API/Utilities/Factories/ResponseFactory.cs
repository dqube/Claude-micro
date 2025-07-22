using Microsoft.AspNetCore.Http;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Utilities.Factories;

public static class ResponseFactory
{
    public static IResult Success<T>(T data, string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Success(data, message, correlationId);
        return Results.Ok(response);
    }

    public static IResult Success(string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Success(message, correlationId);
        return Results.Ok(response);
    }

    public static IResult Created<T>(T data, string? location = null, string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Success(data, message ?? "Resource created successfully", correlationId);
        
        return location != null 
            ? Results.Created(location, response)
            : Results.Created(string.Empty, response);
    }

    public static IResult NoContent(string? correlationId = null)
    {
        return Results.NoContent();
    }

    public static IResult BadRequest(string message, string? correlationId = null)
    {
        var response = ResponseHelper.Error(message, "BAD_REQUEST", correlationId: correlationId);
        return Results.BadRequest(response);
    }

    public static IResult NotFound(string resource, string? correlationId = null)
    {
        var response = ResponseHelper.NotFound(resource, correlationId);
        return Results.NotFound(response);
    }

    public static IResult Unauthorized(string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Unauthorized(message, correlationId);
        return Results.Unauthorized();
    }

    public static IResult Forbidden(string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Forbidden(message, correlationId);
        return Results.Problem(
            detail: response.Message,
            statusCode: 403,
            title: "Forbidden");
    }

    public static IResult ValidationError(IDictionary<string, string[]> errors, string? correlationId = null)
    {
        var response = ResponseHelper.ValidationError("Validation failed", errors, correlationId);
        return Results.ValidationProblem(errors, 
            detail: response.Message,
            title: "Validation Error");
    }

    public static IResult Conflict(string message, string? correlationId = null)
    {
        var response = ResponseHelper.Error(message, "CONFLICT", correlationId: correlationId);
        return Results.Conflict(response);
    }

    public static IResult InternalServerError(string? message = null, string? correlationId = null)
    {
        var response = ResponseHelper.Error(
            message ?? "An internal server error occurred", 
            "INTERNAL_SERVER_ERROR", 
            correlationId: correlationId);
        
        return Results.Problem(
            detail: response.Message,
            statusCode: 500,
            title: "Internal Server Error");
    }

    public static IResult PagedResult<T>(
        IEnumerable<T> data,
        long totalCount,
        int currentPage,
        int pageSize,
        string? message = null,
        string? correlationId = null)
    {
        var response = ResponseHelper.PagedSuccess(data, totalCount, currentPage, pageSize, message, correlationId);
        return Results.Ok(response);
    }
}