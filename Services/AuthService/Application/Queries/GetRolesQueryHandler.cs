using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Queries;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IReadOnlyRepository<Role, RoleId> _roleRepository;

    public GetRolesQueryHandler(IReadOnlyRepository<Role, RoleId> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<RoleDto>> HandleAsync(GetRolesQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        System.Linq.Expressions.Expression<Func<Role, bool>> specification = r => true;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            specification = r => r.Name.ToLower().Contains(searchTerm) ||
                                (r.Description != null && r.Description.ToLower().Contains(searchTerm));
        }

        var roles = await _roleRepository.FindAsync(specification, cancellationToken);

        return roles?.Select(MapToDto).ToList() ?? new List<RoleDto>();
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id.Value,
            Name = role.Name,
            Description = role.Description ?? string.Empty,
            CreatedAt = role.CreatedAt,
            LastUpdatedAt = role.UpdatedAt
        };
    }
}