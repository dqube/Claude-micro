using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;

namespace AuthService.Domain.Entities;

public class User : AggregateRoot<UserId>
{
    public Username Username { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public UserId? UpdatedBy { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    // Private constructor for EF Core
    private User() : base(UserId.New())
    {
        Username = new Username("temp");
        Email = new Email("temp@temp.com");
        PasswordHash = new PasswordHash([1]);
        PasswordSalt = [1];
    }

    public User(
        UserId id,
        Username username,
        Email email,
        PasswordHash passwordHash,
        byte[] passwordSalt) : base(id)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        PasswordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;

        AddDomainEvent(new UserCreatedEvent(Id, Username, Email));
    }

    public void UpdateEmail(Email email, UserId updatedBy)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdatePassword(PasswordHash newPasswordHash, byte[] newPasswordSalt, UserId updatedBy)
    {
        PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
        PasswordSalt = newPasswordSalt ?? throw new ArgumentNullException(nameof(newPasswordSalt));
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        FailedLoginAttempts = 0; // Reset failed attempts on password change
        LockoutEnd = null; // Remove lockout on password change

        AddDomainEvent(new UserPasswordUpdatedEvent(Id));
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        // Lock account after 5 failed attempts for 30 minutes
        if (FailedLoginAttempts >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(30);
        }
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate(UserId activatedBy)
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;
        FailedLoginAttempts = 0;
        LockoutEnd = null;

        AddDomainEvent(new UserActivatedEvent(Id));
    }

    public void Deactivate(UserId deactivatedBy)
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;

        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }

    public bool CanLogin()
    {
        return IsActive && !IsLockedOut();
    }
}