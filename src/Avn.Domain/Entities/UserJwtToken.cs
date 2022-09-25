namespace Avn.Domain.Entities;

[Table(nameof(UserJwtToken), Schema = nameof(EntitySchema.Auth))]
public class UserJwtToken : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserJwtTokenId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string AccessTokenHash { get; set; }

    public DateTimeOffset AccessTokenExpiresTime { get; set; }

    [Required]
    [MaxLength(128)]
    public string RefreshTokenHash { get; set; }

    public DateTimeOffset RefreshTokenExpiresTime { get; set; }
}
