#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using ContactService.Application.Commands;
using ContactService.Application.DTOs;
using ContactService.Application.Queries;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace ContactService.API.Endpoints;

internal static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/contacts")
            .WithTags("Contacts")
            .WithOpenApi();

        group.MapGet("/", GetContactsAsync)
            .WithName("GetContacts")
            .WithSummary("Get all contacts with optional filtering and pagination")
            .WithDescription("Retrieve a paginated list of contacts with optional search and filtering capabilities")
            .Produces<PagedResult<ContactDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetContactByIdAsync)
            .WithName("GetContactById")
            .WithSummary("Get contact by ID")
            .WithDescription("Retrieve a specific contact by their unique identifier")
            .Produces<ContactDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateContactAsync)
            .WithName("CreateContact")
            .WithSummary("Create a new contact")
            .WithDescription("Create a new contact record with the provided information")
            .Produces<ContactDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", UpdateContactAsync)
            .WithName("UpdateContact")
            .WithSummary("Update contact information")
            .WithDescription("Update all information for a specific contact")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteContactAsync)
            .WithName("DeleteContact")
            .WithSummary("Delete a contact")
            .WithDescription("Delete a specific contact by their unique identifier")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetContactsAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetContactsRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            var query = new GetContactsQuery(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.ContactType,
                request.IsActive,
                request.SortBy,
                request.SortDescending);

            var result = await mediator.QueryAsync<GetContactsQuery, PagedResult<ContactDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.PagedResult(result.Items, result.TotalCount, result.Page, result.PageSize, "Contacts retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contacts: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contacts: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contacts: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetContactByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            var query = new GetContactByIdQuery(id);
            var result = await mediator.QueryAsync<GetContactByIdQuery, ContactDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            if (result == null)
            {
                return ResponseFactory.NotFound("Contact", correlationId);
            }
            
            return ResponseFactory.Success(result, "Contact retrieved successfully", correlationId);
        }
        catch (Exception ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound("Contact", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contact: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contact: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve contact: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreateContactAsync(
        [FromBody] CreateContactRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            var command = new CreateContactCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.ContactType,
                request.PhoneNumber,
                request.Address,
                request.Company,
                request.JobTitle,
                request.Notes);

            await mediator.SendAsync<CreateContactCommand>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Success("Contact created successfully", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid contact data: {ex.Message}", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create contact: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create contact: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create contact: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> UpdateContactAsync(
        Guid id,
        [FromBody] UpdateContactRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            var command = new UpdateContactCommand(
                id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.Company,
                request.JobTitle,
                request.Notes);

            await mediator.SendAsync<UpdateContactCommand>(command, cancellationToken);
            
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.Success("Contact updated successfully", correlationId);
        }
        catch (Exception ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound("Contact", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid contact data: {ex.Message}", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update contact: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update contact: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update contact: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> DeleteContactAsync(
        Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            var command = new DeleteContactCommand(id);
            await mediator.SendAsync<DeleteContactCommand>(command, cancellationToken);
            
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.Success("Contact deleted successfully", correlationId);
        }
        catch (Exception ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound("Contact", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to delete contact: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to delete contact: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to delete contact: {ex.Message}", correlationId);
        }
    }
}

internal sealed record GetContactsRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? ContactType = null,
    bool? IsActive = null,
    string? SortBy = null,
    bool SortDescending = false);

internal sealed record CreateContactRequest(
    string FirstName,
    string LastName,
    string Email,
    string ContactType,
    [property: JsonConverter(typeof(FlexibleNullableStringConverter))]
    string? PhoneNumber,
    AddressDto? Address,
    string? Company,
    string? JobTitle,
    string? Notes);

internal sealed record UpdateContactRequest(
    string FirstName,
    string LastName,
    string Email,
    [property: JsonConverter(typeof(FlexibleNullableStringConverter))]
    string? PhoneNumber,
    AddressDto? Address,
    string? Company,
    string? JobTitle,
    string? Notes);