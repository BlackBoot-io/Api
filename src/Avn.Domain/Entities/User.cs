namespace Avn.Domain.Entities;

[Table(nameof(User), Schema = nameof(EntitySchema.Auth))]
public class User : IEntity
{
    public User() => UserId = Guid.NewGuid();

    [Key]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(128)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public bool EmailIsApproved { get; set; }

    [Required]
    [MaxLength(256)]
    public string Password { get; set; }

    [Required]
    [MaxLength(256)]
    public string PasswordSalt { get; set; }

    public UserType Type { get; set; }

    [Required]
    [MaxLength(256)]
    public string WalletAddress { get; set; }

    public DateTime? LockoutEndDateUtc { get; set; }
    public bool IsLockoutEnabled { get; set; }

    public bool IsActive { get; set; }

    [MaxLength(150)]
    public string OrganizationName { get; set; } = null!;

    public ICollection<UserJwtToken> UserJwtTokens { get; set; }
    public ICollection<Project> Projects { get; set; }
    public ICollection<Drop> Drops { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; }
}