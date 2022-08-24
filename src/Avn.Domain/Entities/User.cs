namespace Avn.Domain.Entities;

[Table(nameof(User), Schema = nameof(EntitySchema.Admin))]
public class User : IEntity
{
    public User() => UserId = Guid.NewGuid();


    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid UserId { get; set; }



    [Required, MaxLength(150)]
    public string FullName { get; set; } = null!;

    [Required, MaxLength(128)]
    public string Email { get; set; }

    [Required]
    public bool EmailIsApproved { get; set; }

    [Required, MaxLength(256)]
    public string Password { get; set; }
    public string PasswordSalt { get; set; }

    public UserType UserType { get; set; }

    [MaxLength(150)]
    public string OrganizationName { get; set; } = null!;

    [MaxLength(256)]
    public string WalletAdress { get; set; }

    public bool IsActive { get; set; }

    public ICollection<UserJwtToken> UserJwtTokens { get; set; }
    public ICollection<Project> Projects { get; set; }
    public ICollection<Event> Events { get; set; }
}