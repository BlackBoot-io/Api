namespace Avn.Domain.Entities;

[Table(nameof(VerificationCode), Schema = nameof(EntitySchema.Auth))]
public class VerificationCode : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VerificationId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    public VerificationType VerificationType { get; set; }

    public int PinCode { get; set; }

    public bool IsUsed { get; set; }

    public DateTime InsertDateMi { get; set; }

    public DateTime ExpirationTime { get; set; }
}
