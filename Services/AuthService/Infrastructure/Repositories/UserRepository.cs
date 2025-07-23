using AuthService.Domain.Entities;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : EfRepository<User, UserId>, IRepository<User, UserId>
{
    public UserRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }
}