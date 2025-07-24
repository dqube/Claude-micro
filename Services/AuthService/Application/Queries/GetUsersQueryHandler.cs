using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Queries;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IReadOnlyRepository<User, UserId> _userRepository;

    public GetUsersQueryHandler(IReadOnlyRepository<User, UserId> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Build specification based on filters
        var specification = BuildSpecification(request);

        // Get results using basic find method
        var allUsers = await _userRepository.FindAsync(specification, cancellationToken);
        
        var totalCount = allUsers.Count;
        
        // Apply pagination manually
        var users = allUsers
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var userDtos = users?.Select(MapToDto).ToList() ?? new List<UserDto>();

        return new PagedResult<UserDto>(
            userDtos,
            totalCount,
            request.Page,
            request.PageSize);
    }

    private static System.Linq.Expressions.Expression<Func<User, bool>> BuildSpecification(GetUsersQuery request)
    {
        System.Linq.Expressions.Expression<Func<User, bool>> specification = u => true;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            specification = specification.And(u => 
                u.Username.Value.ToLower().Contains(searchTerm) ||
                u.Email.Value.ToLower().Contains(searchTerm));
        }

        if (request.IsActive.HasValue)
        {
            specification = specification.And(u => u.IsActive == request.IsActive.Value);
        }

        if (request.IsLocked.HasValue)
        {
            specification = specification.And(u => u.IsLockedOut() == request.IsLocked.Value);
        }

        return specification;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            Username = user.Username.Value,
            Email = user.Email.Value,
            IsActive = user.IsActive,
            IsLocked = user.IsLockedOut(),
            FailedLoginAttempts = user.FailedLoginAttempts,
            LastLoginAt = null, // Not tracked in this version
            LockedUntil = user.LockoutEnd,
            CreatedAt = user.CreatedAt,
            LastUpdatedAt = user.UpdatedAt,
            Roles = new List<RoleDto>() // Roles not loaded in list view for performance
        };
    }
}

// Extension method for combining expressions
internal static class ExpressionExtensions
{
    public static System.Linq.Expressions.Expression<Func<T, bool>> And<T>(
        this System.Linq.Expressions.Expression<Func<T, bool>> expr1,
        System.Linq.Expressions.Expression<Func<T, bool>> expr2)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T));
        
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);
        
        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
            System.Linq.Expressions.Expression.AndAlso(left!, right!), parameter);
    }
}

internal class ReplaceExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
{
    private readonly System.Linq.Expressions.Expression _oldValue;
    private readonly System.Linq.Expressions.Expression _newValue;

    public ReplaceExpressionVisitor(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override System.Linq.Expressions.Expression? Visit(System.Linq.Expressions.Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}