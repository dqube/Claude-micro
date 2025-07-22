using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Commands;
using PatientService.Application.DTOs;
using PatientService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Mediator;

namespace PatientService.API.Endpoints;

public static class PatientEndpoints
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
        CancellationToken cancellationToken = default)
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
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPatientByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetPatientByIdQuery(id);
            var result = await mediator.QueryAsync<GetPatientByIdQuery, PatientDto>(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound(new { Message = ex.Message });
        }
    }

    private static async Task<IResult> CreatePatientAsync(
        [FromBody] CreatePatientRequest request,
        [FromServices] IMediator mediator,
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
            
            return Results.Created($"/api/patients/{result.Id}", result);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { Message = ex.Message });
        }
    }

    private static async Task<IResult> UpdatePatientContactAsync(
        Guid id,
        [FromBody] UpdateContactRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdatePatientContactCommand(id, request.Email, request.PhoneNumber);
            await mediator.SendAsync<UpdatePatientContactCommand>(command, cancellationToken);
            
            return Results.NoContent();
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { Message = ex.Message });
        }
    }
}

// Request DTOs for minimal API
public record GetPatientsRequest(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? IsActive = null,
    string? Gender = null,
    int? MinAge = null,
    int? MaxAge = null);

public record CreatePatientRequest(
    string MedicalRecordNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string? PhoneNumber,
    AddressDto? Address,
    DateTime DateOfBirth,
    string Gender,
    string? BloodType = null);

public record UpdateContactRequest(
    string Email,
    string? PhoneNumber = null);