using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;

namespace AuthService.Application.Queries;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<RoleDto>> HandleAsync(GetRolesQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IEnumerable<Role> roles;

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            // Use domain-specific method for better performance when getting all roles
            roles = await _roleRepository.GetAllOrderedByNameAsync(cancellationToken);
        }
        else
        {
            // Use search functionality for filtering
            var searchTerm = request.SearchTerm.ToLower();
            System.Linq.Expressions.Expression<Func<Role, bool>> specification = r => 
                r.Name.ToLower().Contains(searchTerm) ||
                (r.Description != null && r.Description.ToLower().Contains(searchTerm));
            
            roles = await _roleRepository.FindAsync(specification, cancellationToken);
        }

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