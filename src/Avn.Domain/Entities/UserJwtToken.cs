namespace Avn.Domain.Entities;

[Table(nameof(UserJwtToken), Schema = nameof(EntitySchema.Admin))]
public class UserJwtToken : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserJwtTokenId { get; set; }

    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    public string AccessTokenHash { get; set; }
    public DateTimeOffset AccessTokenExpiresTime { get; set; }
    public string RefreshTokenHash { get; set; }
    public DateTimeOffset RefreshTokenExpiresTime { get; set; }
}
