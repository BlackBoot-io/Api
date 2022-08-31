namespace Avn.Domain.Entities;

[Table(nameof(Project), Schema = nameof(EntitySchema.Base))]
public class Project : IEntity
{
    public Project() => Id = Guid.NewGuid();

    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    [Required]
    [MaxLength(15)]
    [Column(TypeName = "varchar")]
    public string SourceIp { get; set; }

    [Required]
    [MaxLength(50)]
    public string ApiKey { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar")]
    public string Website { get; set; }

    public DateTime InsertDate { get; set; }
    public ICollection<Event> Events { get; set; }
}
