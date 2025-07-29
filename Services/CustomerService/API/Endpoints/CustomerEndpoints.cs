#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using CustomerService.Application.Commands;
using CustomerService.Application.DTOs;
using CustomerService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace CustomerService.API.Endpoints;

internal static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers")
            .WithTags("Customers")
            .WithOpenApi();

        // GET /api/customers
        group.MapGet("/", GetCustomersAsync)
            .WithName("GetCustomers")
            .WithSummary("Get all customers with optional filtering and pagination")
            .WithDescription("Retrieve a paginated list of customers with optional search and filtering capabilities")
            .Produces<PagedResult<CustomerDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/customers/{id}
        group.MapGet("/{id:guid}", GetCustomerByIdAsync)
            .WithName("GetCustomerById")
            .WithSummary("Get customer by ID")
            .WithDescription("Retrieve a specific customer by their unique identifier")
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/customers
        group.MapPost("/", CreateCustomerAsync)
            .WithName("CreateCustomer")
            .WithSummary("Create a new customer")
            .WithDescription("Create a new customer account with the provided information")
            .Produces<CustomerDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetCustomersAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetCustomersRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetCustomersQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.CountryCode,
                request.IsMembershipActive);

            var result = await mediator.QueryAsync<GetCustomersQuery, PagedResult<CustomerDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.PagedResult(result.Items, result.TotalCount, result.Page, result.PageSize, "Customers retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve customers: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request cancelled: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request timeout: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetCustomerByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await mediator.QueryAsync<GetCustomerByIdQuery, CustomerDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "Customer retrieved successfully", correlationId)
                : ResponseFactory.NotFound("Customer", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve customer: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request cancelled: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request timeout: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreateCustomerAsync(
        [FromBody] CreateCustomerRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateCustomerCommand(
                request.FirstName,
                request.LastName,
                request.CountryCode,
                request.UserId,
                request.Email,
                request.MembershipNumber,
                request.JoinDate,
                request.ExpiryDate);

            var result = await mediator.SendAsync<CreateCustomerCommand, CustomerDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/customers/{result.Id}", "Customer created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create customer: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal sealed record GetCustomersRequest(
    [property: JsonPropertyName("page")] int Page = 1,
    [property: JsonPropertyName("pageSize")] int PageSize = 10,
    [property: JsonPropertyName("searchTerm")] string? SearchTerm = null,
    [property: JsonPropertyName("countryCode")] string? CountryCode = null,
    [property: JsonPropertyName("isMembershipActive")] bool? IsMembershipActive = null
);

internal sealed record CreateCustomerRequest(
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("countryCode")] string CountryCode,
    [property: JsonPropertyName("userId")] Guid? UserId,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("membershipNumber")] string? MembershipNumber,
    [property: JsonPropertyName("joinDate")] DateTime? JoinDate,
    [property: JsonPropertyName("expiryDate")] DateTime? ExpiryDate
); 