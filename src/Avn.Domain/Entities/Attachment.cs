namespace Avn.Domain.Entities;

[Table(nameof(Attachment), Schema = nameof(EntitySchema.File))]
public class Attachment : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid? UserId { get; set; }

    [Required]
    public byte[] Content { get; set; }

    public DateTime InsertDate { get; set; }

    public ICollection<Drop> Drops { get; set; }
}
