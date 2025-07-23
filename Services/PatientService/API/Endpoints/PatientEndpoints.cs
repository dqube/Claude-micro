using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Commands;
using PatientService.Application.DTOs;
using PatientService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace PatientService.API.Endpoints;

internal static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/patients")
            .WithTags("Patients")
            .WithOpenApi();

        // GET /api/patients
        group.MapGet("/", GetPatientsAsync)
            .WithName("GetPatients")
            .WithSummary("Get all patients with optional filtering and pagination")
            .WithDescription("Retrieve a paginated list of patients with optional search and filtering capabilities")
            .Produces<PagedResult<PatientDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/patients/{id}
        group.MapGet("/{id:guid}", GetPatientByIdAsync)
            .WithName("GetPatientById")
            .WithSummary("Get patient by ID")
            .WithDescription("Retrieve a specific patient by their unique identifier")
            .Produces<PatientDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/patients
        group.MapPost("/", CreatePatientAsync)
            .WithName("CreatePatient")
            .WithSummary("Create a new patient")
            .WithDescription("Create a new patient record with the provided information")
            .Produces<PatientDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // PUT /api/patients/{id}/contact
        group.MapPut("/{id:guid}/contact", UpdatePatientContactAsync)
            .WithName("UpdatePatientContact")
            .WithSummary("Update patient contact information")
            .WithDescription("Update the email and phone number for a specific patient")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetPatientsAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetPatientsRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetPatientsQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.IsActive,
                request.Gender,
                request.MinAge,
                request.MaxAge);

            var result = await mediator.QueryAsync<GetPatientsQuery, PagedResult<PatientDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.PagedResult(result.Items, result.TotalCount, result.Page, result.PageSize, "Patients retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patients: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patients: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patients: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetPatientByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetPatientByIdQuery(id);
            var result = await mediator.QueryAsync<GetPatientByIdQuery, PatientDto>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Success(result, "Patient retrieved successfully", correlationId);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound("Patient", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patient: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patient: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve patient: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreatePatientAsync(
        [FromBody] CreatePatientRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreatePatientCommand(
                request.MedicalRecordNumber,
                request.FirstName,
                request.LastName,
                request.MiddleName,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.DateOfBirth,
                request.Gender,
                request.BloodType);

            var result = await mediator.SendAsync<CreatePatientCommand, PatientDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Created(result, $"/api/patients/{result.Id}", "Patient created successfully", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid patient data: {ex.Message}", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create patient: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create patient: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create patient: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> UpdatePatientContactAsync(
        Guid id,
        [FromBody] UpdateContactRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdatePatientContactCommand(id, request.Email, request.PhoneNumber);
            await mediator.SendAsync<UpdatePatientContactCommand>(command, cancellationToken);
            
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.Success("Patient contact updated successfully", correlationId);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound("Patient", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid contact data: {ex.Message}", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update patient contact: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update patient contact: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update patient contact: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs for minimal API
internal record GetPatientsRequest(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? IsActive = null,
    string? Gender = null,
    int? MinAge = null,
    int? MaxAge = null);

internal record CreatePatientRequest(
    string MedicalRecordNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    [property: JsonConverter(typeof(FlexibleNullableStringConverter))]
    string? PhoneNumber,
    AddressDto? Address,
    DateTime DateOfBirth,
    string Gender,
    string? BloodType = null);

internal record UpdateContactRequest(
    string Email,
    [property: JsonConverter(typeof(FlexibleNullableStringConverter))]
    string? PhoneNumber = null);