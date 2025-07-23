using BuildingBlocks.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Events;

namespace AuthService.Domain.Entities;

public class User : AggregateRoot<UserId>
{
    public Username Username { get; private set; }
    public Email Email { get; private set; }
    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }
    public bool IsActive { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RegistrationToken> _registrationTokens = new();
    public IReadOnlyCollection<RegistrationToken> RegistrationTokens => _registrationTokens.AsReadOnly();

    // Private constructor for EF Core
    private User() { }

    private User(UserId id, Username username, Email email, byte[] passwordHash, byte[] passwordSalt)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        IsActive = true;
        FailedLoginAttempts = 0;

        AddDomainEvent(new UserCreatedEvent(Id.Value, Username.Value, Email.Value));
    }

    public static User Create(Username username, Email email, byte[] passwordHash, byte[] passwordSalt)
    {
        return new User(UserId.New(), username, email, passwordHash, passwordSalt);
    }

    public void AddRole(Role role)
    {
        if (!_userRoles.Any(ur => ur.RoleId == role.Id))
        {
            _userRoles.Add(new UserRole(Id, role.Id));
        }
    }

    public void AddRegistrationToken(TokenType tokenType, DateTime expiration)
    {
        _registrationTokens.Add(RegistrationToken.Create(Id, tokenType, expiration));
    }

    public void SetPassword(byte[] passwordHash, byte[] passwordSalt)
    {
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void IncrementFailedLoginAttempts()
    {
        FailedLoginAttempts++;
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
    }

    public void LockUser(DateTime lockoutEnd)
    {
        LockoutEnd = lockoutEnd;
        IsActive = false;
    }

    public void UnlockUser()
    {
        LockoutEnd = null;
        IsActive = true;
    }
}
